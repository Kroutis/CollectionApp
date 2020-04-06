document.write('\
    @foreach(var comment in Model.ItemComments)\
{\
    @if (comment.ItemId == Model.Item.Id) \
    { \
    <p>@comment.UserName : @comment.Text</p>\
}\
}\
    ');