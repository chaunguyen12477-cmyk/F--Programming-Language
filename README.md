
# F-- Programming Language 🚀

[![Build Status](https://github.com/realmg51-cpu/F---Programming-Language/workflows/.NET%20Build/badge.svg)]
[![Test Status](https://github.com/realmg51-cpu/F--Programming-Language/workflows/.NET%20Test/badge.svg)]
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)]
[![NuGet Version](https://img.shields.io/nuget/v/FSharpMinus)]

## 🌟 Giới thiệu
**F--** (F Minus Minus) - "The backward step of humanity, but forward step in creativity!"

Được tạo bởi **lập trình viên 13 tuổi**, F-- là ngôn ngữ lập trình độc đáo với triết lý:
> "Cứ code đi, compiler lo phần còn lại!"

## ✨ Tính năng nổi bật
- ✅ **Import cả computer** - Không cần quan tâm phần cứng!
- ✅ **String interpolation** - `$"Hello {name}!"`
- ✅ **Quản lý bộ nhớ tự động**
- ✅ **File I/O siêu trực quan** - `at "file.txt" { ... }`
- ✅ **Tự động báo lỗi FMM** - F Minus Minus Error codes

## 🎮 Ví dụ nhanh
```f--
import system
using namespace sys

start()
{
    name = "F--"
    version = 1.4
    
    println($"Xin chào từ {name} v{version}!")
    println($"Memory còn: {memory.memoryleft}")
    
    io.cfile("hello"(path "txt"))
    at "hello.txt"
    {
        io.println("Hello file!")
        io.save()
    }
    
    return(0) // Thành công!
}
```

🚀 Cài đặt & Chạy

```bash
# Clone repo
git clone https://github.com/yourusername/FSharpMinus.git

# Build
dotnet build

# Chạy file F--
dotnet run -- run examples/hello.f--

# Xem AST tree
dotnet run -- ast examples/hello.f--
```

📦 Cài đặt qua NuGet

```bash
dotnet add package FSharpMinus
```

🧪 Chạy tests

```bash
dotnet test
```

📜 License

MIT - Feel free to use, modify, and share!

👨‍💻 Tác giả

Lập trình viên 13 tuổi - Đam mê sáng tạo và code sạch!

---

⭐ Star repo này nếu bạn thấy F-- thú vị!
