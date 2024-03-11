﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace IlluminatedComments
{
    internal static class CommentImageParser
    {
        private static readonly Regex _fSharpImageCommentRegex;
        private static readonly Regex _csharpImageCommentRegex;
        private static readonly Regex _vbImageCommentRegex;
        private static readonly Regex _xmlImageTagRegex;
        private static readonly Regex _pythonImageCommentRegex;

        static CommentImageParser()
        {
            var xmlImageTagPattern = @"-image.*;";

            var cSharpCommentPattern = @"/{2,3}.*";
            _csharpImageCommentRegex = new Regex(cSharpCommentPattern + xmlImageTagPattern, RegexOptions.Compiled);

            var vbCommentPattern = @"'.*";
            _vbImageCommentRegex = new Regex(vbCommentPattern + xmlImageTagPattern, RegexOptions.Compiled);

            var fSharpCommentPattern = @"\(\*.*";
            _fSharpImageCommentRegex = new Regex(fSharpCommentPattern + xmlImageTagPattern, RegexOptions.Compiled);

            var pythonCommentPattern = @"#.*";
            _pythonImageCommentRegex = new Regex(pythonCommentPattern + xmlImageTagPattern, RegexOptions.Compiled);

            _xmlImageTagRegex = new Regex(xmlImageTagPattern, RegexOptions.Compiled);
        }

        /// <summary>
        ///     Tries to match Regex on input text
        /// </summary>
        /// <returns>Position in line at start of matched image comment. -1 if not matched</returns>
        public static int Match(string contentTypeName, string lineText, out string matchedText)
        {
            matchedText = null;
            var result = -1;

            if (!string.IsNullOrEmpty(lineText))
            {
                Match match = null;
                switch (contentTypeName)
                {
                    case ContentTypes.Cpp:
                    case ContentTypes.CSharp:
                    case ContentTypes.Java:
                    case ContentTypes.JavaScript:
                    case ContentTypes.TypeScript:
                        match = _csharpImageCommentRegex.Match(lineText);
                        break;

                    case ContentTypes.VisualBasic:
                        match = _vbImageCommentRegex.Match(lineText);
                        break;

                    case ContentTypes.FSharp:
                        // match // <image or /// <image
                        match = _csharpImageCommentRegex.Match(lineText);
                        if (match == null || string.IsNullOrEmpty(match.Value))
                            // match (*
                            match = _fSharpImageCommentRegex.Match(lineText);
                        if (match == null || string.IsNullOrEmpty(match.Value))
                            // just match <image, could be in a multi-line comment
                            match = _xmlImageTagRegex.Match(lineText);
                        break;

                    case ContentTypes.Python:
                        match = _pythonImageCommentRegex.Match(lineText);
                        break;

                    default:
                        Console.WriteLine("Unsupported content type: " + contentTypeName);
                        break;
                }

                if (match != null)
                {
                    matchedText = match.Value;
                    if (!string.IsNullOrEmpty(matchedText))
                        result = match.Index;
                }
            }

            return result;
        }

        /// <summary>
        ///     Looks for well formed image comment in line of text and tries to parse parameters
        /// </summary>
        /// <param name="matchedText">Input: Line of text in editor window</param>
        /// <param name="imageUrl">Output: URL of image</param>
        /// <param name="imageScale">Output: Scale factor of image </param>
        /// <param name="ex">Instance of any exception generated. Null if function finished succesfully</param>
        /// <returns>Returns true if successful, otherwise false</returns>
        public static bool TryParse(string matchedText, out string imageUrl, out double imageScale, out Exception exception)
        {
            exception = null;
            imageUrl = null;
            imageScale = 0;

            // Try parse text
            if (!string.IsNullOrEmpty(matchedText))
            {
                var tagText = _xmlImageTagRegex.Match(matchedText).Value;
                try
                {
                    var ts = tagText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (ts.Length == 0)
                        return false;
                    Dictionary<string, string> tags = new Dictionary<string, string>();
                    foreach (var tag in ts)
                    {
                        string tagKey = string.Empty, tagValue = string.Empty;
                        bool enableGetValue = false;
                        int index = 0;
                        for (int i = 0; i < tag.Length; i++)
                        {
                            if (tag[i] == '=' && !enableGetValue)
                            {
                                tagKey = tag.Substring(index, i - index);
                            }
                            else if (tag[i] == '\'')
                            {
                                if (enableGetValue)
                                {
                                    enableGetValue = false;
                                    tagValue = tag.Substring(index, i - index);
                                }
                                else
                                {
                                    enableGetValue = true;
                                    index = i;
                                    index++;
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(tagKey) || string.IsNullOrEmpty(tagValue))
                            continue;
                        tags.Add(tagKey, tagValue);
                    }
                    if (!tags.TryGetValue("src", out string srcValue) && !tags.TryGetValue("url", out srcValue))
                    {
                        exception = new XmlException("'url' or 'src' attribute not specified.");
                        return false;
                    }

                    imageUrl = srcValue;

                    if (tags.TryGetValue("scale", out string scaleStr))
                        if (!double.TryParse(scaleStr, NumberStyles.Float, CultureInfo.InvariantCulture, out imageScale))
                            imageScale = 1;

                    return true;
                }
                catch (Exception ex)
                {
                    exception = ex;
                    return false;
                }
            }

            return false;
        }
    }
}