using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace HeapProject.TagHelpers
{
    [HtmlTargetElement("span", Attributes = "asp-display-name-for")]
    public class DisplayNameTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-display-name-for")]
        public ModelExpression For { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.Content.SetContent(For.Metadata.DisplayName ?? For.Name);
        }
    }
}
