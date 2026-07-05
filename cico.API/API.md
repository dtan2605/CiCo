# CICO API Reference

## Kiến trúc

```
Client → [JWT Auth + Rate Limit] → Controllers → MediatR → Handlers → Repositories → EF Core → SQL Server
                                                                        ↕
                                                                  SignalR Hubs (real-time)
                                                                  Hangfire Jobs (background)
```

- **API layer**: Controllers xử lý HTTP, trích xuất userId từ JWT
- **Application layer**: CQRS với MediatR commands/queries + AutoMapper
- **Infrastructure layer**: EF Core DbContext, Repositories, SignalR Hubs, Hangfire Jobs
- **Domain layer**: Entities, ValueObjects, Enums, DomainException

---

## Authentication

Tất cả endpoints (trừ login/register) yêu cầu `Authorization: Bearer <token>`.

### `POST /api/auth/login`

```
Body: { email, password }
→ 200: { accessToken, refreshToken, expiresAt }
→ 401: { error }
```

### `POST /api/auth/register`

```
Body: { email, password, fullName, employeeCode, departmentId, positionId }
→ 200: { accessToken, refreshToken }
```

### `POST /api/auth/refresh-token`

```
Body: { accessToken, refreshToken }
→ 200: { accessToken, refreshToken }
```

### `POST /api/auth/logout`

```
Body: { refreshToken }
→ 204
```

---

## Authorization Policies

| Policy | Allowed Roles | Ghi chú |
|---|---|---|
| `AdminOnly` | Admin | Xoá dữ liệu, cấu hình hệ thống |
| `ManagerOnly` | Manager | Quản lý đội nhóm |
| `StaffOnly` | Employee | Quyền cơ bản |
| `ManagerOrAdmin` | Admin, Manager | CRUD operations |

---

## Rate Limiting

| Policy | Requests/min | Queue | Áp dụng cho |
|---|---|---|---|
| `EmployeePolicy` | 100 | 10 | GET endpoints (mặc định) |
| `WritePolicy` | 20 | 0 | POST/PUT/DELETE |
| `DeletePolicy` | 10 | 0 | DELETE nhạy cảm |
| `LoginPolicy` | 5 | 0 | Auth login |

Response: `429 Too Many Requests`

---

## Modules & Endpoints

### 1. Employee (`/api/employee`)

| Method | Path | Auth | Chức năng |
|---|---|---|---|
| GET | `/` | Authorize | Paging (keyword, departmentId, positionId) |
| GET | `/{id}` | Authorize | Chi tiết employee + department + position |
| POST | `/` | ManagerOrAdmin | Tạo mới |
| PUT | `/` | ManagerOrAdmin | Cập nhật |
| DELETE | `/{id}` | AdminOnly | Xoá |

### 2. Auth (`/api/auth`)

| Method | Path | Auth | Chức năng |
|---|---|---|---|
| POST | `/login` | Anonymous | Đăng nhập, trả JWT |
| POST | `/register` | Anonymous | Đăng ký |
| POST | `/refresh-token` | Anonymous | Refresh JWT |
| POST | `/logout` | Authorize | Logout |

### 3. Schedule (`/api/schedule`)

| Method | Path | Auth | Chức năng |
|---|---|---|---|
| GET | `/` | Authorize | Paging (keyword) |
| GET | `/{id}` | Authorize | Chi tiết ca làm việc |
| POST | `/` | ManagerOrAdmin | Tạo ca mới (Name, StartTime, EndTime, LateThreshold) |
| PUT | `/` | ManagerOrAdmin | Cập nhật |
| DELETE | `/{id}` | AdminOnly | Xoá |

### 4. EmployeeSchedule (`/api/employeeschedule`)

| Method | Path | Auth | Chức năng |
|---|---|---|---|
| GET | `/` | Authorize | Paging (employeeId, scheduleId, fromDate, toDate) |
| GET | `/{id}` | Authorize | Chi tiết |
| POST | `/` | ManagerOrAdmin | Gán lịch cho nhân viên |
| PUT | `/` | ManagerOrAdmin | Cập nhật |
| DELETE | `/{id}` | AdminOnly | Xoá |

### 5. Attendance (`/api/attendance`)

| Method | Path | Auth | Chức năng |
|---|---|---|---|
| GET | `/{id}` | Authorize | Chi tiết điểm danh |
| GET | `/by-employee/{id}` | Authorize | Lịch sử điểm danh của employee |
| GET | `/by-date` | Authorize | Điểm danh theo ngày |
| POST | `/check-in` | ManagerOrAdmin | Check-in (tính LateMinutes tự động dựa trên schedule) |
| PUT | `/check-out` | ManagerOrAdmin | Check-out |

### 6. AttendanceLog (`/api/attendancelog`)

| Method | Path | Auth | Chức năng |
|---|---|---|---|
| GET | `/` | Authorize | Paging (attendanceId, deviceId, fromDate, toDate, isSuccess) |
| GET | `/{id}` | Authorize | Chi tiết log |
| GET | `/by-attendance/{id}` | Authorize | Logs của 1 attendance |
| GET | `/by-device/{id}` | Authorize | Logs từ 1 thiết bị |
| POST | `/` | AdminOnly | Tạo log (từ máy quét) |

### 7. Department (`/api/department`)

| Method | Path | Auth | Chức năng |
|---|---|---|---|
| GET | `/` | Authorize | Danh sách phòng ban |
| GET | `/{id}` | Authorize | Chi tiết |
| POST | `/` | AdminOnly | Tạo mới |
| PUT | `/` | AdminOnly | Cập nhật |
| DELETE | `/{id}` | AdminOnly | Xoá |

### 8. Position (`/api/position`)

| Method | Path | Auth | Chức năng |
|---|---|---|---|
| GET | `/` | Authorize | Danh sách chức vụ |
| GET | `/{id}` | Authorize | Chi tiết |
| POST | `/` | AdminOnly | Tạo mới |
| PUT | `/` | AdminOnly | Cập nhật |
| DELETE | `/{id}` | AdminOnly | Xoá |

### 9. Notification (`/api/notification`)

| Method | Path | Auth | Chức năng |
|---|---|---|---|
| GET | `/` | Authorize | Paging (isRead, type) — tự lọc theo userId từ JWT |
| GET | `/unread` | Authorize | Chưa đọc |
| POST | `/` | AdminOnly | Tạo notification cho employee |
| PUT | `/{id}/read` | Authorize | Đánh dấu đã đọc |
| PUT | `/read-all` | Authorize | Đánh dấu tất cả đã đọc |
| DELETE | `/{id}` | Authorize | Xoá |

### 10. BiometricProfile (`/api/biometricprofile`)

| Method | Path | Auth | Chức năng |
|---|---|---|---|
| GET | `/` | Authorize | Paging (employeeId, type, isActive) |
| GET | `/{id}` | Authorize | Chi tiết |
| GET | `/by-employee/{id}` | Authorize | Profile của 1 employee |
| POST | `/register` | AdminOnly | Đăng ký/update template vân tay/khuôn mặt |
| POST | `/verify` | Authorize | Xác thực template (so sánh với template đã lưu) |
| DELETE | `/{id}` | AdminOnly | Xoá profile |

### 11. Report (`/api/report`)

| Method | Path | Auth | Chức năng |
|---|---|---|---|
| GET | `/dashboard?date=...` | Authorize | Tổng quan: total employees, present/absent today |
| GET | `/daily/{date}` | Authorize | Chi tiết trong ngày: danh sách employee kèm check-in/out |
| GET | `/weekly?fromDate=...&toDate=...` | Authorize | Báo cáo tuần: daily summaries |
| GET | `/monthly/{year}/{month}` | Authorize | Báo cáo tháng: daily summaries + attendance rate |

### 12. User (`/api/user`)

*(Dành cho quản lý người dùng hệ thống)*

| Method | Path | Auth | Chức năng |
|---|---|---|---|
| GET | `/` | Authorize | Paging |
| GET | `/{id}` | Authorize | Chi tiết user + roles |

---

## SignalR Hubs

### `/hub/dashboard` — DashboardHub

Yêu cầu `[Authorize]`. Client gửi token qua query string: `/hub/dashboard?access_token=...`

| Client -> Server | Mô tả |
|---|---|
| `JoinDashboardGroup` | Tham gia group Dashboard (nhận cập nhật real-time) |
| `LeaveDashboardGroup` | Rời group |

| Server -> Client | Payload | Điều kiện |
|---|---|---|
| `DashboardUpdated` | `{ totalEmployees, presentToday, absentToday }` | Trong group Dashboard |

### `/hub/notifications` — NotificationHub

Tự động join group theo `ClaimTypes.NameIdentifier` khi kết nối.

| Server -> Client | Payload | Điều kiện |
|---|---|---|
| `ReceiveNotification` | `{ title, content, type, createdAt }` | User ID trùng với group |

---

## Background Jobs (Hangfire)

Dashboard: `GET /hangfire` (Admin-only)

| Job | Schedule | Cron | Chức năng |
|---|---|---|---|
| `AttendanceReminderJob` | 8:00 AM daily | `0 8 * * *` | Nhắc nhở check-in cho employee chưa điểm danh |
| `NotificationCleanupJob` | 2:00 AM Chủ nhật | `0 2 * * 0` | Xoá notifications > 30 ngày |

---

## Error Handling

- `DomainException` → `400 Bad Request` với `{ error: "message" }`
- Validation errors → `400 Bad Request` với FluentValidation errors
- Unhandled exceptions → `500 Internal Server Error` (dev) / generic error (prod)
- 401 Unauthorized → thiếu/ sai JWT token
- 403 Forbidden → không đủ role
- 429 Too Many Requests → vượt rate limit

---

## Response Convention

- `200 OK` + body: Thành công (GET)
- `201 Created` + body + `Location` header: Tạo mới (POST)
- `204 No Content`: Cập nhật / Xoá (PUT, DELETE)
- `400 Bad Request`: DomainException hoặc validation lỗi
- `404 Not Found`: Resource không tồn tại
