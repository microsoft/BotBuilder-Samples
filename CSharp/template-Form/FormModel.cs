namespace FormTemplate
{
    using System;
    using Microsoft.Bot.Builder.FormFlow;

    // POCO model used to build FormFlow Dialog
    // This class represents the Formto be filled
    [Serializable]
    public class FormModel
    {
        [Prompt("Hi! What is your {&}?")]
        public string Name { get; set; }

        [Prompt("Please say something and I'll repeat it")]
        public string Message { get; set; }

        public static IForm<FormModel> BuildForm()
        {
            // Builds an IForm<T> based on FormModel POCO
            return new FormBuilder<FormModel>().Build();
        }

        public static IFormDialog<FormModel> BuildFormDialog()
        {
            // Generated a new FormDialog<T> based on IForm<FormModel>
            return FormDialog.FromForm(FormModel.BuildForm);
        }
    }
}