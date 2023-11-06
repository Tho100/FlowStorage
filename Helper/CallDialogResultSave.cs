using System.Windows.Forms;

namespace FlowSERVER1.Helper {
    public class CallDialogResultSave {
        public static DialogResult CallDialogResult(bool isFromSharing) {

            string verifySaveText = null;

            if (isFromSharing) {
                verifySaveText = "Save Changes? \nThe changes will also affect the user you've shared this file to.";

            } else {
                verifySaveText = "Save Changes?";

            }

            DialogResult verifySave = MessageBox.Show(verifySaveText, "Flowstorage", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk);
            return verifySave;
        
        }
    }
}
