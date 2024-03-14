# Illuminated Comments

## 目录

1. [生效范围](#coverage)
2. [安装方法](#installation)
3. [使用方法](#usage)
4. [提示](#reminders)
5. [Credit](#credit)

## 生效范围

### Visual Studio
- 经测试，Vsiual Studio 2022 Community
- 理论上能支持 Visual Studio 2017-2022

### 图像格式
理论上 WPF 支持的格式都能生效，如：
- `.png`
- `.jpg`
- `.gif`

### 编程语言
- C#
- VisualBasic
- C/C++
- ConsoleOutput
- CSS
- ENC
- FindResults
- F#
- HTML
- JavaScript
- XAML
- XML
- TypeScript
- Python
- Java

## 安装方法
1. 从[发布页](https://github.com/zqzcsilver/IlluminatedComments/releases)下载 `.vsix` 文件
2. 双击 `.vsix` 开始安装

## 使用方法

在注释中插入 `-image,url='路径/或/URL',scale='数';`。

### 示例

- 插入带缩放的本地图像;
```
class Program
{
   /// <summary>
   /// <para>查看图片了解更多：</para>
   /// <para>-image,url='Resources/Test.png',scale='1.2';</para>
   /// </summary>
   static void Test()
   {
   }
}
```

- 插入不带缩放的网页图像。
```
class Program
{
   /// <summary>
   /// <para>查看图片了解更多：</para>
   /// <para>-image,url='https://visualstudio.microsoft.com/wp-content/uploads/2022/11/vs-icon.svg';</para>
   /// </summary>
   static void Test()
   {
   }
}
```

### Parameters
| 名称 | 必填 | 描述 | 示例 |
| :-------------: | :-------------: | :-------------: | :-------------: |
| `url`/`src`  | 是  | **图像源。**<br>可以是路径或 URL；绝对和相对路径皆可。 | `url='$(ProjectDir)Test.png'`<br>`src='D:/Test.png'`<br>`url='https://test/test.png'`
| `scale`  | 否 | **图像缩放。**<br>请注意，极度放大可能导致显示问题。 | `scale='3'`<br>`scale ='0.2'` |

### 环境变量
`$(SolutionDir)` 和 `$(ProjectDir)`，这两个 Visual Studio 的环境变量可内插进参数 `url`/`src`。

## 提示
- 将图像注释单独放进一个 `<para></para>` 标签里，因为同一标签内的其他注释不会显示；
- 图像 URL 前请加上 `https://` 或 `http://`。

## 差异
这与 [risadams](https://github.com/risadams) 的 [Illuminated-comments](https://github.com/risadams/Illuminated-comments) 之间有什么区别？
-  [Illuminated-comments](https://github.com/risadams/Illuminated-comments) 仅会在源代码页面显示图片注释，而快速信息窗口和自动补全窗口不会。
-  [Illuminated-comments](https://github.com/risadams/Illuminated-comments) 并不如其预期一样工作，它在读取图片后不会解除对文件的占用，导致其他应用无法读取或删除图片。

## Credit
此扩展是基于 [risadams](https://github.com/risadams) 的 [Illuminated-comments](https://github.com/risadams/Illuminated-comments) 开发的。

特别感谢 [Lacewing](https://github.com/lace-wing)，没有他的帮助，我不可能更新得这么快。
