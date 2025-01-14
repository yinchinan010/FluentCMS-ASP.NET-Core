﻿namespace FluentCMS.Web.Plugins;

public partial class FormActions
{
    [Inject]
    protected NavigationManager NavigationManager { get; set; } = default!;

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public bool Submit { get; set; }

    [Parameter]
    public bool Cancel { get; set; }

    protected virtual string GetBackUrl()
    {
        return new Uri(NavigationManager.Uri).LocalPath;
    }
}
