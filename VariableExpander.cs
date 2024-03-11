using System;
using System.IO;
using System.Text.RegularExpressions;

using EnvDTE;

using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace IlluminatedComments
{
    /// <summary>
    ///     This class provides variable substitution for strings, e.g. replacing '$(ProjectDir)' with 'C:\MyProject\'
    ///     Currently supported variables are:
    ///     $(ProjectDir)
    ///     $(SolutionDir)
    /// </summary>
    public class VariableExpander
    {
        private const string VARIABLE_PATTERN = @"\$\(\S+?\)";

        private const string PROJECTDIR_PATTERN = "$(ProjectDir)";
        private const string SOLUTIONDIR_PATTERN = "$(SolutionDir)";
        private readonly Regex _variableMatcher;

        private readonly IWpfTextView _view;

        private string _projectDirectory;
        private readonly SVsServiceProvider _serviceProvider;
        private string _solutionDirectory;

        public VariableExpander(IWpfTextView view, SVsServiceProvider serviceProvider)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            _variableMatcher = new Regex(VARIABLE_PATTERN, RegexOptions.Compiled);
            try
            {
                populateVariableValues();
            }
            catch (Exception ex)
            {
                ExceptionHandler.Notify(ex, true);
            }
        }

        /// <summary>
        ///     Processes URL by replacing $(Variables) with their values
        /// </summary>
        /// <param name="urlString">Input URL string</param>
        /// <returns>Processed URL string</returns>
        public string ProcessText(string urlString)
        {
            var processedText = _variableMatcher.Replace(urlString, evaluator);
            return processedText;
        }

        /// <summary>
        ///     Regex.Replace Match evaluator callback. Performs variable name/value substitution
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        private string evaluator(Match match)
        {
            var variableName = match.Value;
            if (string.Compare(variableName, PROJECTDIR_PATTERN, StringComparison.InvariantCultureIgnoreCase) == 0)
                return _projectDirectory;
            if (string.Compare(variableName, SOLUTIONDIR_PATTERN, StringComparison.InvariantCultureIgnoreCase) == 0)
                return _solutionDirectory;
            return variableName;
        }

        /// <summary>
        ///     Populates variable values from the ProjectItem associated with the TextView.
        /// </summary>
        /// <remarks>
        ///     Based on code from http://stackoverflow.com/a/2493865
        ///     Guarantees variables will not be null, but they may be empty if e.g. file isn't part of a project, or solution
        ///     hasn't been saved yet
        ///     TODO: If additional variables are added that reference the path to the document, handle cases of 'Save as' /
        ///     renaming
        /// </remarks>
        private void populateVariableValues()
        {
            _projectDirectory = "";
            _solutionDirectory = "";

            _view.TextDataModel.DocumentBuffer.Properties.TryGetProperty(typeof(ITextDocument), out ITextDocument document);

            var dte = (DTE)_serviceProvider.GetService(typeof(DTE));
            var projectItem = dte.Solution.FindProjectItem(document.FilePath);

            if (projectItem != null && projectItem.ContainingProject != null)
            {
                var projectPath = projectItem.ContainingProject.FileName;
                if (projectPath != "") // projectPath will be empty if file isn't part of a project.
                    _projectDirectory = Path.GetDirectoryName(projectPath) + @"\";

                var solutionPath = dte.Solution.FileName;
                if (solutionPath != "") // solutionPath will be empty if project isn't part of a saved solution
                    _solutionDirectory = Path.GetDirectoryName(solutionPath) + @"\";
            }
        }
    }
}