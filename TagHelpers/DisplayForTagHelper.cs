using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HeapProject.TagHelpers
{
    [HtmlTargetElement("span", Attributes = "asp-display-for")]
    public class DisplayForTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-display-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.SetContent(For.Model?.ToString() ?? string.Empty);
        }
    }
}
