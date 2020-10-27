using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace ExecuteCMD
{
    class DataBase : INotifyPropertyChanged
    {
        private int dbId;
        private bool isCheck;
        private bool upgraded;
        private string status;

        public int DBId
        {
            get { return dbId; }
            set
            {
                dbId = value;
                OnPropertyChanged("DBId");
            }
        }

        public bool IsCheck
        {
            get { return isCheck; }
            set
            {
                isCheck = value;
                OnPropertyChanged("IsCheck");
            }
        }

        public bool Upgraded
        {
            get { return upgraded; }
            set
            {
                upgraded = value;
                OnPropertyChanged("Upgraded");
            }
        }
        public string Status
        {
            get { return status; }
            set
            {
                status = value;
                OnPropertyChanged("Status");
            }
        }

        public string DBName { get; set; }

        public string DBPath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
