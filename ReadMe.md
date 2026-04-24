# L4D2 Voice Lines

求生之路 2 幸存者台词收集工具。

台词已记录在 [L4D2.VoiceLines.txt](https://github.com/frg2089/L4D2.VoiceLines.txt) 项目中了

## 功能

- 提取游戏中的幸存者语音音频文件
- 使用 Whisper AI 进行语音转文字

## 系统要求

- Windows 11 24H2 或更高版本
- Steam 版求生之路 2
- .NET 10.0 SDK

## 项目结构

```
src/
  L4D2.VoiceLines.Collector/    # C# 语音转文字工具

```

## 快速开始

### 1. 克隆项目

```bash
git clone https://github.com/frg2089/L4D2.VoiceLines.git
cd L4D2.VoiceLines
```

### 2. 构建语音收集器

```bash
dotnet build
```

### 3. 运行语音收集

自动检测 Steam 安装路径：
```bash
dotnet run --project src/L4D2.VoiceLines.Collector
```

或指定游戏路径：
```bash
dotnet run --project src/L4D2.VoiceLines.Collector "D:\Steam\steamapps\common\Left 4 Dead 2"
```

输出的 `.index` 文件位于 `dist/` 目录，格式为：
```
文件名|转录文本
```

## 支持的幸存者

| 角色 | 文件夹 |
|------|--------|
| Coach | coach |
| Nick | gambler |
| Ellis | mechanic |
| Zoey | teengirl |
| Francis | biker |
| Louis | manager |
| Bill | namvet |
| Rochelle | producer |

## 技术栈

- **C# / .NET 10** - 语音转文字核心
- **Microsoft AI Foundry Local** - WinML 本地推理
- **OpenAI Whisper** - 语音识别

## 许可证

[MIT License](LICENSE)
