using Microsoft.AspNetCore.Components;

namespace FluentCMS.Web.UI.Components;

public abstract class BaseComponent : ComponentBase
{
    [Parameter]
    public bool Visible { get; set; } = true;

    [Parameter]
    public RenderFragment ChildContent { get; set; } = default!;

    [Parameter]
    public string? Class { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> AdditionalAttributes { get; set; } = default!;

    [Parameter]
    public string? CSSName { get; set; }

    public virtual string GetDefaultCSSName()
    {
        var type = GetType();
        if (type.IsGenericType)
            return type.Name.Split("`").First().FromPascalCaseToKebabCase();
        else
            return type.Name.FromPascalCaseToKebabCase();
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        parameters.SetParameterProperties(this);

        if (Visible)
            return base.SetParametersAsync(parameters);

        return Task.CompletedTask;
    }

    // Prevents rendering if StateHasChanged is called
    protected override bool ShouldRender()
    {
        return Visible;
    }
}
