# Illuminated Comments

## Table of Content

1. [Coverage](#coverage)
2. [Installation](#installation)
3. [Usage](#usage)
4. [Reminders](#reminders)
5. [Credit](#credit)

## Coverage

### Visual Studio
- tested with Vsiual Studio 2022 Community
- technically Visual Studio 2017-2022 are also supported

### Image Format
Technically all formats supported by WPF, such as:
- `.png`
- `.jpg`
- `.gif`

### Programming Languages
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

## Installation
1. download `.vsix` file from the [release page](https://github.com/zqzcsilver/IlluminatedComments/releases)
2. double click on it to install

## Usage

Insert `-image,url='path/or/url',scale='number';` in comments.

### Examples

- inserting a local image with scaling
```
class Program
{
   /// <summary>
   /// <para>Learn more from this picture:</para>
   /// <para>-image,url='Resources/Test.png',scale='2';</para>
   /// </summary>
   static void Test()
   {
   }
}
```

- inserting a web image without scaling
```
class Program
{
   /// <summary>
   /// <para>Learn more from this picture:</para>
   /// <para>-image,url='https://visualstudio.microsoft.com/wp-content/uploads/2022/11/vs-icon.svg';</para>
   /// </summary>
   static void Test()
   {
   }
}
```

### Parameters
| Name | Required | Description | Example |
| :-------------: | :-------------: | :-------------: | :-------------: |
| `url`/`src`  | yes  | **Source of the image.**<br>It can be a path or an URL; both absolute and relative path are accepted. | `url='$(ProjectDir)Test.png'`<br>`src='D:/Test.png'`<br>`url='https://test/test.png'`
| `scale`  | no | **Scale of the image.**<br>Note that a very large scale may cause display issues. | `scale='3'`<br>`scale ='0.2'` |

### Environment Variables
Two Visual Studio environment variables, namely `$(SolutionDir)` and `$(ProjectDir)` can be interpolated in parameter `url`/`src`.

## Reminders
- place the image comment in a separate `<para></para>` tag, because rest of the comment in that tag will not be displayed
- precede web image URL with `https://`/`http://`

## Credit
This extension is made based on [Illuminated-comments](https://github.com/risadams/Illuminated-comments) by [risadams](https://github.com/risadams).

Special thanks to [Lacewing](https://github.com/lace-wing), I wouldn't have updated the extension so quickly without his help.
