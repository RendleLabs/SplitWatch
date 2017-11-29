namespace RendleLabs.Diagnostics.SplitWatchFormat
{
    internal static class HtmlTemplate
    {
        public const string Css = @".split { border-width: 0; min-height: 20px; }
    .root { background-color: lightblue; margin-left: 240px; }
    .split-label { font-family: monospace; font-size: 10pt; position: absolute; width: 236px; }
    .split-label span { text-overflow: ellipsis; overflow: hidden; display: inline-block; white-space: nowrap; color: black; }
    .level-0>.split-label { left: 4px; width: 236px; }
    .level-1>.split-label { left: 8px; width: 232px; }
    .level-2>.split-label { left: 12px; width: 228px; }
    .level-3>.split-label { left: 16px; width: 224px; }
    .level-4>.split-label { left: 20px; width: 220px; }
    .level-0>.split-label>span { width: 200px; }
    .level-1>.split-label>span { width: 196px; }
    .level-2>.split-label>span { width: 192px; }
    .level-3>.split-label>span { width: 188px; }
    .level-4>.split-label>span { width: 184px; }
    .split:hover>.split-label { font-weight: bold; }
    .spacer { height: 16px; font-family: sans-serif; font-size: 9pt; padding-top: 4px; padding-left: 4px }
    .collapsed>.sub { display: none; }
    button.subs-0 { visibility: hidden; }
    .toggle-button { width: 16px; height: 16px; border: solid 1px #333333; background-color: white; font-size: 8pt; border-radius: 8px; color: #333333; transform: rotate(90deg);  }
    .collapsed .toggle-button { transform: rotate(0deg); }";

        // From Paletton: http://paletton.com/palette.php?uid=72M120kllllaFw0g0qFqFg0w0aF
        public const string Palette = @".palette-0 .level-0 { background-color: #7986AC; color: black; } 
.palette-0 .level-1 { background-color: #50608F; color: white; } 
.palette-0 .level-2 { background-color: #2F4073; color: white; } 
.palette-0 .level-3 { background-color: #162656; color: white; } 
.palette-0 .level-4 { background-color: #061339; color: white; } 
.palette-1 .level-0 { background-color: #F8A6AC; } 
.palette-1 .level-1 { background-color: #CF676F; color: white; } 
.palette-1 .level-2 { background-color: #A6373F; color: white; } 
.palette-1 .level-3 { background-color: #7C151D; color: white; } 
.palette-1 .level-4 { background-color: #530006; color: white; } 
.palette-2 .level-0 { background-color: #97D38D; } 
.palette-2 .level-1 { background-color: #64B058; color: white; } 
.palette-2 .level-2 { background-color: #3C8D2F; color: white; } 
.palette-2 .level-3 { background-color: #1E6912; color: white; } 
.palette-2 .level-4 { background-color: #0A4600; color: white; } 
.palette-3 .level-0 { background-color: #FFE4AA; } 
.palette-3 .level-1 { background-color: #D4B36A; color: white; } 
.palette-3 .level-2 { background-color: #AA8639; color: white; } 
.palette-3 .level-3 { background-color: #805E15; color: white; } 
.palette-3 .level-4 { background-color: #553A00; color: white; } ";

        public const string Js = @"document.addEventListener('DOMContentLoaded', () => {
    for (const btn of document.querySelectorAll('.toggle-button')) {
      btn.addEventListener('click', (e) => {
        e.currentTarget.parentElement.parentElement.classList.toggle('collapsed');
      });
    }
  });";
    }
}