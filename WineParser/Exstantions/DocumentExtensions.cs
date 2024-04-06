using AngleSharp.Dom;

namespace WineParser.Exstantions
{
    public static class DocumentExtensions
    {
        public static IElement QuerySelectorSafe(this IDocument document, string selector)
        {
            var element = document.QuerySelector(selector);
            if (element == null)
            {
                throw new InvalidOperationException($"Element matching selector '{selector}' not found.");
            }
            return element;
        }
    }
}
