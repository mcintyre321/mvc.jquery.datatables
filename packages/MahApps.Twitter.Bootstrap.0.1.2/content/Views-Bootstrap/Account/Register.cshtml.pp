@model $rootnamespace$.Models.RegisterModel
@{
    ViewBag.Title = "Register";
}
<div class="content">
    <div class="page-header">
        <h2>
            Create a New Account</h2>
    </div>
    <p>
        Use the form below to create a new account.
    </p>
    <p>
        Passwords are required to be a minimum of @Membership.MinRequiredPasswordLength
        characters in length.
    </p>
    <script src="@Url.Content("~/Scripts/jquery.validate.min.js")" type="text/javascript"></script>
    <script src="@Url.Content("~/Scripts/jquery.validate.unobtrusive.min.js")" type="text/javascript"></script>
    @using (Html.BeginForm())
    {
        @Html.ValidationSummary(true, "Account creation was unsuccessful. Please correct the errors and try again.")
        <div>
            <fieldset>
                <legend>Account Information</legend>
                <div class="clearfix">
                    @Html.LabelFor(m => m.UserName)
                    <div class="input">
                        @Html.TextBoxFor(m => m.UserName)
                        @Html.ValidationMessageFor(m => m.UserName)
                    </div>
                </div>
                <div class="clearfix">
                    @Html.LabelFor(m => m.Email)
                    <div class="input">
                        @Html.TextBoxFor(m => m.Email)
                        @Html.ValidationMessageFor(m => m.Email)
                    </div>
                </div>
                <div class="clearfix">
                    @Html.LabelFor(m => m.Password)
                    <div class="input">
                        @Html.PasswordFor(m => m.Password)
                        @Html.ValidationMessageFor(m => m.Password)
                    </div>
                </div>
                <div class="clearfix">
                    @Html.LabelFor(m => m.ConfirmPassword)
                    <div class="input">
                        @Html.PasswordFor(m => m.ConfirmPassword)
                        @Html.ValidationMessageFor(m => m.ConfirmPassword)
                    </div>
                </div>
            </fieldset>
        </div>
        <div class="actions">
            <input type="submit" class="btn primary" value="Register" />
        </div>
    }
</div>
