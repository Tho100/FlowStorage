using FlowSERVER1.Global;
using FlowSERVER1.Sharing;
using System.Threading;

namespace FlowSERVER1.Helper {
    public class StartPopupForm {

        public static void StartUploadingFilePopup(string fileName, long fileSize = 0) {
            UploadingFilePopup(fileName, fileSize);
        }

        public static void StartUploadingFolderPopup(string folderName) {
            UploadingFolderPopup(folderName);
        }

        public static void StartRetrievalPopup(bool isFromLogin = false) {
            RetrievalPopup(isFromLogin);
        }

        public static void StartSharingPopup(string receiverUsername) {
            SharingPopup(receiverUsername);
        }

        private static void SharingPopup(string receiverUsername) {
            new Thread(() => new SharingAlert(
                shareToName: receiverUsername).ShowDialog()).Start();

        }

        private static void RetrievalPopup(bool origin) {
            new Thread(() => new RetrievalAlert(
                "Retrieving your files...", origin).ShowDialog()).Start();
            
        }

        private static void UploadingFilePopup(string fileName, long fileSizeInMb = 0) {
            new Thread(() => new UploadingAlert(
                fileName, string.Empty, string.Empty, fileSize: fileSizeInMb).ShowDialog()).Start();

        }

        private static void UploadingFolderPopup(string folderName) {
            new Thread(() => new UploadingAlert(
                folderName, GlobalsTable.folderUploadTable, folderName).ShowDialog()).Start();

        }

    }
}
