using HarmonyLib;

using System.IO;
using System.Reflection;
using System.Text;

namespace IlluminatedComments.Patches
{
    internal class HarmonyLoader
    {
        public static Harmony Harmony;
        public static Assembly VSEditorAssembly;
        public static Assembly PresentationCoreAssembly;

        public static void Load()
        {
            if (Harmony != null)
                return;

            AssemblyName assemblyName = new AssemblyName("Microsoft.CodeAnalysis.EditorFeatures");
            assemblyName.KeyPair = new StrongNameKeyPair("31bf3856ad364e35");
            PresentationCoreAssembly = Assembly.Load(assemblyName);
            assemblyName = new AssemblyName("Microsoft.VisualStudio.Platform.VSEditor");
            assemblyName.KeyPair = new StrongNameKeyPair("b03f5f7f11d50a3a");
            VSEditorAssembly = Assembly.Load(assemblyName);

            Harmony = new Harmony("IlluminatedComments");
            Harmony.Patch(VSEditorAssembly.GetType("Microsoft.VisualStudio.Language.Intellisense.Implementation.AsyncQuickInfoSession")
                .GetMethod("ComputeContentAsync", BindingFlags.NonPublic | BindingFlags.Instance),
                null, new HarmonyMethod(typeof(AsyncQuickInfoSessionPatch).GetMethod("ComputeContentAsyncPostfix")));

            Harmony.Patch(PresentationCoreAssembly.GetType("Microsoft.CodeAnalysis.Editor.Implementation.IntelliSense.AsyncCompletion.CompletionSource")
                .GetMethod("GetDescriptionAsync"), null,
                typeof(CompletionSourcePatch).GetMethod("GetDescriptionAsyncPostfix"));

            //Harmony.Patch(VSEditorAssembly.GetType("Microsoft.VisualStudio.Language.Intellisense.Implementation.SignatureHelpSessionView")
            //    .GetMethod("DisplayContent", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public),
            //    typeof(SignatureHelpSessionViewPatch).GetMethod("DisplayContentPrefix"));

            //Harmony.Patch(VSEditorAssembly.GetType("Microsoft.VisualStudio.Language.Intellisense.Implementation.SignatureHelpSessionView")
            //    .GetMethod("DisplayContent", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public),
            //    typeof(SignatureHelpSessionViewPatch).GetMethod("DisplayPrettyPrintedContentPrefix"));
        }
    }
}