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
using SimpleInjector;

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
        private int _totalPages;
        private int _currentPage;
        private IPagedList<Log> _currentSet;

        /// <summary>
        /// Ctor
        /// </summary>
        public LoggingViewModel()
        {
            if (!IsInDesignMode)
            {
                ConnectCommand = new RelayCommand(Connect);
                ForwardCommand = new RelayCommand(Forward, () => _currentSet != null && _currentSet.HasPreviousPage);
                BackCommand = new RelayCommand(Back, () => _currentSet != null && _currentSet.HasNextPage);
            }
        }

        private void Back()
        {
            throw new NotImplementedException();
        }

        private void Forward()
        {
            throw new NotImplementedException();
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
        public RelayCommand ConnectCommand { get; protected set; }

        /// <summary>
        /// Go back
        /// </summary>
        public RelayCommand BackCommand { get; protected set; }

        /// <summary>
        /// Go forward
        /// </summary>
        public RelayCommand ForwardCommand { get; protected set; }

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
                Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                IsConnected = false;
            }
        }

        /// <summary>
        /// The total number of pages
        /// </summary>
        public int TotalPages
        {
            get { return _totalPages; }
            set
            {
                _totalPages = value;
                RaisePropertyChanged(() => TotalPages);
            }
        }

        /// <summary>
        /// The current page
        /// </summary>
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                if (_totalPages <= 0)
                    _currentPage = -1;
                else
                {
                    if (value < 0)
                    {
                        _currentPage = 1;
                    }
                    else if (value > TotalPages)
                    {
                        _currentPage = TotalPages;
                    }
                    else
                    {
                        _currentPage = TotalPages;
                    }
                }
                RaisePropertyChanged(() => CurrentPage);
            }
        }

        public void Refresh()
        {
            if (_currentSet == null || (_currentPage - 1) != _currentSet.PageIndex)
            {
                _currentSet = new DefaultLogger(new Container(), _logCollection).GetAllLogs(pageIndex: _currentPage);
                _currentPage = _currentSet.PageIndex + 1;
                TotalPages = _currentSet.TotalPages;
                _logs.Clear();
                foreach(var log in _currentSet)
                {
                    _logs.Add(log);
                }
            }
        }
    }
}
