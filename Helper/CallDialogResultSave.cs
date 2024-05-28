using System.Windows.Forms;

namespace FlowstorageDesktop.Helper {
    public class CallDialogResultSave {
        public static DialogResult CallDialogResult(bool isFromSharing) {

            string saveText = isFromSharing 
                ? "Save Changes? \nThe changes will also affect the user you've shared this file to."
                : "Save Changes?";

            return MessageBox.Show(
                saveText, "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
        
        }
    }
}
