
using System;
using System.ComponentModel;

namespace Sample.LogService
{
    internal class LogEntry : INotifyPropertyChanged
    {
        private int _index;
        private DateTime _originalTimestamp;
        private DateTime _correctedTimestamp;
        private ClientType _clientType;
        private string _threadId;
        private string _logger;
        private string _message;

        public int Index
        {
            get
            {
                return _index;
            }
            set
            {
                if (_index != value)
                {
                    _index = value;
                    RaisePropertyChangedEvent("Index");
                }
            }
        }

        public DateTime OriginalTimestamp
        {
            get
            {
                return _originalTimestamp;
            }
            set
            {
                if (_originalTimestamp != value)
                {
                    _originalTimestamp = value;
                    RaisePropertyChangedEvent("OriginalTimestamp");
                }
            }
        }

        public DateTime CorrectedTimestamp
        {
            get
            {
                return _correctedTimestamp;
            }
            set
            {
                if (_correctedTimestamp != value)
                {
                    _correctedTimestamp = value;
                    RaisePropertyChangedEvent("CorrectedTimestamp");
                }
            }
        }

        public ClientType ClientType
        {
            get
            {
                return _clientType;
            }
            set
            {
                if (_clientType != value)
                {
                    _clientType = value;
                    RaisePropertyChangedEvent("ClientType");
                }
            }
        }

        public string ThreadId
        {
            get
            {
                return _threadId;
            }
            set
            {
                if (_threadId != value)
                {
                    _threadId = value;
                    RaisePropertyChangedEvent("ThreadId");
                }
            }
        }

        public string Logger
        {
            get
            {
                return _logger;
            }
            set
            {
                if (_logger != value)
                {
                    _logger = value;
                    RaisePropertyChangedEvent("Logger");
                }
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (_message != value)
                {
                    _message = value;
                    RaisePropertyChangedEvent("Message");
                }
            }
        }

        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        private void RaisePropertyChangedEvent(string propertyName)
        {
            var handlers = PropertyChanged;
            if (handlers != null)
            {
                handlers(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
