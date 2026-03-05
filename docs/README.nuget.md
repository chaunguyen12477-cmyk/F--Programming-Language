# F-- Programming Language

[![NuGet Version](https://img.shields.io/nuget/v/Fminusminus)](https://www.nuget.org/packages/Fminusminus/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Fminusminus)](https://www.nuget.org/packages/Fminusminus/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![GitHub Stars](https://img.shields.io/github/stars/realmg51-cpu/F---Programming-Language?style=social)](https://github.com/realmg51-cpu/F---Programming-Language)

---

## 🚀 What is F--?

**F--** (F Minus Minus) is a unique programming language created by a **13-year-old developer**. 

> *"The backward step of humanity, but forward step in creativity!"*

Built with **.NET 8**, F-- features a simple syntax, automatic memory management, and intuitive file I/O operations - perfect for learning compiler design or just having fun with a new language!

---

## ✨ Features

| Feature | Description |
|---------|-------------|
| 🖥️ **Super Import** | Just `import computer` and you're ready! |
| 🔤 **String Interpolation** | Use `$"Hello {name}!"` for dynamic strings |
| 🧠 **Memory Management** | Built-in commands like `memory.memoryleft` |
| 📁 **Intuitive File I/O** | Work with files using `at "file.txt" { ... }` |
| 🚨 **FMM Error Codes** | Professional error system (F Minus Minus) |

---

## 📦 Installation

### Install as Global Tool

```bash
dotnet tool install --global Fminusminus --version 2.0.0
```

Add to Project

```bash
dotnet add package Fminusminus --version 2.0.0
```

---

🎮 Quick Start

Create a file hello.f--:

```f--
import computer
start()
{
    name = "F--"
    version = 2.0
    
    println($"Hello from {name} v{version}!")
    println($"Memory left: {memory.memoryleft} MB")
    
    return(0)
    end()
}
```

Run it:

```bash
# If installed as global tool
fminus run hello.f--

# If using dotnet run
dotnet run --project Compiler/compiler.csproj -- run hello.f--
```

Output:

```
Hello from F-- v2.0!
Memory left: 768 MB
```

---

📖 Examples

1. Hello World

```f--
import computer
start()
{
    println("Hello, World!")
    return(0)
    end()
}
```

2. File I/O

```f--
import computer
start()
{
    io.cfile("test"(path "txt"))
    at "test.txt"
    {
        io.println("F-- is awesome!")
        io.println($"Created by F-- v{version}")
        io.save()
    }
    println("File created!")
    return(0)
    end()
}
```

3. Memory Check

```f--
import computer
start()
{
    println($"Memory left: {memory.memoryleft}")
    println($"Memory used: {memory.memoryused}")
    println($"Total memory: {memory.memorytotal}")
    return(0)
    end()
}
```

---

📚 Documentation

Basic Syntax Rules

1. Every program must start with import computer
2. Entry point is start() followed by { ... }
3. Every program must end with return(0) and end()
4. Statements can end with optional ;

Available Commands

Command Description Example
print() Print without newline print("Hello")
println() Print with newline println("Hello")
memory.memoryleft Check available memory println(memory.memoryleft)
io.cfile() Create file io.cfile("test"(path "txt"))
at File context block at "file.txt" { ... }
io.save() Save current file io.save()

---

🛠️ Development

Prerequisites

· .NET SDK 8.0 or higher

Build from source

```bash
git clone https://github.com/realmg51-cpu/F---Programming-Language.git
cd F---Programming-Language
dotnet build Compiler/compiler.csproj
```

Run tests

```bash
dotnet test Test/tests.csproj
```

---

🤝 Contributing

We welcome contributions! Special thanks to:

· realmg51-cpu - Creator and lead developer (13 years old!)
· chaunguyen12477-cmyk - Contributor and test engineer

How to contribute

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

---

📜 License

This project is licensed under the MIT License - see the LICENSE file for details.

```
MIT License

Copyright (c) 2026 RealMG

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction...
```

---

👨‍💻 About the Creator

RealMG is a 13-year-old developer passionate about programming language design and compiler construction.

"Age is just a number when it comes to creativity and passion!"

---

⭐ Support

If you like F--, please:

· ⭐ Star the GitHub repository
· 🍴 Fork it and experiment
· 📢 Share with your friends

---

🚀 Coming Soon

· More examples and tutorials
· Language Server Protocol (LSP) support
· VS Code extension
· Package more features!

---

<div align="center">
  <sub>Built with ❤️ by a 13-year-old developer and friends</sub>
  <br/>
  <sub>📅 Last updated: March 2026</sub>
</div>
```

---
