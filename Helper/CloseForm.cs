using System.Linq;
using System.Windows.Forms;

namespace FlowSERVER1 {
    public class CloseForm {

        private const string uploadingAlert = "UploadingAlert";
        private const string retrievalAlert = "RetrievalAlert";
        private const string sharingAlert = "SharingAlert";

        public static void CloseCustomPopup(string formName) {
            ClosePopup(formName);
        }

        public static void CloseUploadingPopup() {
            ClosePopup(uploadingAlert);
        }

        public static void CloseRetrievalPopup() {
            ClosePopup(retrievalAlert);
        }

        public static void CloseSharingPopup() {
            ClosePopup(sharingAlert);
        }

        private static void ClosePopup(string formName) {
            var retrievalAlertForm = Application.OpenForms
             .OfType<Form>()
             .FirstOrDefault(form => form.Name == formName);
            retrievalAlertForm?.Close();
        }

    }
}
