using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

using System;
using System.Text;

namespace IlluminatedComments.Patches
{
    internal class SignatureHelpSessionViewPatch
    {
        public static bool DisplayContentPrefix(object __instance, ISignature sigToRender)
        {
            var _signatureWpfTextView = (IWpfTextView)__instance.GetType().GetField("_signatureWpfTextView", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance);
            _signatureWpfTextView?.Options.SetOptionValue(DefaultTextViewOptions.WordWrapStyleId, WordWrapStyles.None);
            var _signatureTextBuffer = (ITextBuffer)__instance.GetType().GetField("_signatureTextBuffer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance);
            if (_signatureTextBuffer != null)
            {
                _signatureTextBuffer.Properties["UsePrettyPrintedContent"] = false;
                _signatureTextBuffer.Replace(new Span(0, _signatureTextBuffer.CurrentSnapshot.Length), DeleteSignText(sigToRender.Content));
            }
            return false;
        }

        public static bool DisplayPrettyPrintedContentPrefix(object __instance, ISignature sigToRender)
        {
            var _signatureWpfTextView = (IWpfTextView)__instance.GetType().GetField("_signatureWpfTextView", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance);
            _signatureWpfTextView?.Options.SetOptionValue(DefaultTextViewOptions.WordWrapStyleId, WordWrapStyles.None);
            var _signatureTextBuffer = (ITextBuffer)__instance.GetType().GetField("_signatureTextBuffer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(__instance);
            if (_signatureTextBuffer != null)
            {
                _signatureTextBuffer.Properties["UsePrettyPrintedContent"] = true;
                _signatureTextBuffer.Replace(new Span(0, _signatureTextBuffer.CurrentSnapshot.Length), DeleteSignText(sigToRender.PrettyPrintedContent));
            }
            return false;
        }

        private static string DeleteSignText(string text)
        {
            var lines = text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
            StringBuilder sb = new StringBuilder();
            foreach (var line in lines)
            {
                if (!CommentImageParser.TryParse(line, out _, out _, out _))
                    sb.AppendLine(line);
            }
            return sb.ToString();
        }
    }
}