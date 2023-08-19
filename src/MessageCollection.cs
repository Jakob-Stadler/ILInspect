using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace ILInspect {

    public class MessageCollection : INotifyPropertyChanged {
        private StringBuilder messageBuilder = new();
        public event PropertyChangedEventHandler? PropertyChanged;
        public Control? control;

        public MessageCollection() {
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") {
            try {
                if (this.control?.IsHandleCreated ?? false) {
                    this.control?.Invoke(() => { PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); });
                }
            }
            catch (InvalidOperationException) {
                // Ignore if not invokable
            }
        }

        public void addLine(string message) {
            lock(messageBuilder) {
                this.messageBuilder.AppendLine(message);
            }
            this.NotifyPropertyChanged("Text");
        }

        public void addMessage(string message) {
            lock (messageBuilder) {
                this.messageBuilder.Append(message);
            }
            this.NotifyPropertyChanged("Text");
        }

        public string Text {
            get {
                lock (messageBuilder) {
                    return this.messageBuilder.ToString();
                }
            }
        }
    }
}
