using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MongoDB.Driver;
using Noodle.Data;

namespace Noodle.Logging.Viewer
{
    public class LoggingViewModel : ViewModelBase
    {
        private ObservableCollection<Log> _logs = new ObservableCollection<Log>();
        private string _connectionString;
        private string _databaseName;
        private RelayCommand _connectCommand;
        private MongoCollection<Log> _logCollection;
        private MongoDatabase _database;
        private bool _isConnected;

        /// <summary>
        /// Ctor
        /// </summary>
        public LoggingViewModel()
        {
            if (!IsInDesignMode)
            {
                ConnectCommand = new RelayCommand(Connect);
            }
        }

        public ObservableCollection<Log> Logs
        {
            get { return _logs; }
        }

        /// <summary>
        /// The server we will be connecting to
        /// </summary>
        public string ConnectionString
        {
            get { return _connectionString; }
            set
            {
                _connectionString = value;
                RaisePropertyChanged(() => ConnectionString);
            }
        }

        /// <summary>
        /// The database name on the server we will use
        /// </summary>
        public string DatabaseName
        {
            get { return _databaseName; }
            set
            {
                _databaseName = value;
                RaisePropertyChanged(() => DatabaseName);
            }
        }

        /// <summary>
        /// Are we connected to a database?
        /// </summary>
        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                RaisePropertyChanged(() => IsConnected);
            }
        }

        /// <summary>
        /// Estabish a connection to the server
        /// </summary>
        public RelayCommand ConnectCommand
        {
            get { return _connectCommand; }
            set { _connectCommand = value; }
        }

        /// <summary>
        /// Try to connect to the server
        /// </summary>
        private void Connect()
        {
            try
            {
                _database = new MongoDB.MongoService(new SqlConnectionProvider(ConnectionString)).GetDatabase(databaseName: DatabaseName);
                _logCollection = _database.GetCollection<Log>("Log");
                IsConnected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                IsConnected = false;
            }
        }

        
    }
}
