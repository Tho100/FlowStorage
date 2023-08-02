using System.Linq;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public class CloseForm {
        public static void closeForm(string formName) {
            var retrievalAlertForm = Application.OpenForms
             .OfType<Form>()
             .FirstOrDefault(form => form.Name == formName);
            retrievalAlertForm?.Close();
        }
    }
}
