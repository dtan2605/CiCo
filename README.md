# CiCo — Check-in / Check-out Attendance System

Hệ thống chấm công nội bộ, hỗ trợ check-in/check-out qua web, đồng bộ thiết bị Hikvision DS-K1T343MFX/DS-K1T341CMFW, quản lý ca làm việc, nghỉ phép, và báo cáo.

---

## Tech Stack

| Layer | Công nghệ |
|---|---|
| Backend | .NET 10, ASP.NET Core, MediatR (CQRS), EF Core + SQL Server, Hangfire, SignalR |
| Frontend | Next.js 16 (App Router), Tailwind CSS 4, lucide-react |
| Auth | JWT (access + refresh), Policy-based (Admin/Manager/Employee) |
| DevOps | Docker, Docker Compose, Jenkins |

---

## Architecture

```
cico-web/        → Next.js 16 (port 3000)
cico.API/        → ASP.NET Core (port 5009)
cico.Application → CQRS (Commands / Queries / Handlers)
cico.Domain/     → Entities + Enums
cico.Infrastructure → EF Core, Repositories, Hangfire Jobs, Device Integration
```

Backend theo pattern **CQRS với MediatR**: mỗi request là một Command/Query riêng, Handler xử lý nghiệp vụ, repository truy cập DB. Migration tự động khi chạy lần đầu.

---

## Modules

### Employee
- Dashboard: check-in / check-out, today's overview
- Schedule: xem lịch ca theo tuần
- Attendance Log: lịch sử check-in/check-out
- Profile: thông tin cá nhân, avatar, gửi yêu cầu cập nhật
- Leave Requests: gửi đơn nghỉ phép
- Notifications: thông báo hệ thống

### Manager
- Dept Employees: danh sách nhân viên trong phòng ban
- Attendance Log: lịch sử chấm công của phòng ban

### Admin
- Users: quản lý tài khoản (CRUD, phân quyền)
- Schedules: quản lý ca làm việc + gán ca hàng loạt (DayOfWeek mask)
- Devices: quản lý thiết bị Hikvision (CRUD, test connection, sync-all)
- Attendance Log: xem + cancel bản ghi chấm công
- Profile Requests: duyệt/từ chối yêu cầu cập nhật profile
- Leave Requests: duyệt/từ chối đơn nghỉ phép
- Schedule Requests: duyệt/từ chối yêu cầu đổi ca
- Notifications: gửi thông báo broadcast

---

## Chạy Local (Dev)

### Backend
```bash
# Cần SQL Server, connection string trong appsettings.json
cd cico.API
dotnet run
# → http://localhost:5009
```

### Frontend
```bash
cd cico-web
npm install
npm run dev
# → http://localhost:3000
```

---

## Docker

```bash
# 1. Tạo file .env từ mẫu, sửa DB_CONNECTION
cp .env.example .env

# 2. Build & chạy
docker compose up -d
# → API: http://localhost:5009
# → Web: http://localhost:3000
```

---

## DevOps (Jenkins)

```bash
# Khởi động Jenkins
docker compose -f ci/docker-compose.jenkins.yml up -d
# → http://localhost:8081

# Pipeline (Jenkinsfile): Build Docker images → docker compose up -d
```
