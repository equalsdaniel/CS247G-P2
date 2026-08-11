import type { Metadata } from "next";
import "./globals.css";

export const metadata: Metadata = {
  title: "记忆侦探：别墅谋杀案",
  description: "在暴雨夜的旧别墅中询问嫌疑人、读取记忆并破解一场密室谋杀。",
  openGraph: {
    title: "记忆侦探：别墅谋杀案",
    description: "进入暴雨夜的旧别墅，询问五名嫌疑人并还原被篡改的记忆。",
    images: [{ url: "/og.png", width: 1672, height: 941, alt: "记忆侦探：别墅谋杀案" }],
  },
  twitter: {
    card: "summary_large_image",
    title: "记忆侦探：别墅谋杀案",
    description: "进入暴雨夜的旧别墅，询问五名嫌疑人并还原被篡改的记忆。",
    images: ["/og.png"],
  },
};

export default function RootLayout({ children }: Readonly<{ children: React.ReactNode }>) {
  return <html lang="zh-CN"><body>{children}</body></html>;
}
