import type { Metadata } from "next";
import "@fontsource/inter/400.css";
import "@fontsource/inter/500.css";
import "@fontsource/schibsted-grotesk/400.css";
import "@fontsource/schibsted-grotesk/700.css";
import "@fontsource/schibsted-grotesk/900.css";
import "./globals.css";

export const metadata: Metadata = {
  title: "CiCo | Attendance System",
  description: "Simple, elegant check-in check-out system",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en" className="h-full">
      <body className="h-full flex flex-col antialiased">
        {children}
      </body>
    </html>
  );
}
