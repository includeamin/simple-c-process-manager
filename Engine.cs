public class Engine:INotifyPropertyChanged
    {
        public string Exe { get; set; }
        public string Path { get; set; }
        public string Status { get; set; }
        public int Pid { get; set; }
        public string InternalError { get; set; }
        public string ExeError { get; set; }

        private readonly Process _process;
        public string OutPut { get; set; }

        public Engine(string exe,string path)
        {
            Exe = exe;
            Path = path;
            var info = new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = exe,
                Arguments = path
            };
            _process = Process.Start(info);

        }

        public void Start()
        {
            try
            {
                _process.StartInfo.RedirectStandardOutput = true;
                _process.OutputDataReceived += ProcessOnOutputDataReceived;
                _process.ErrorDataReceived += ProcessOnErrorDataReceived;
                _process.StartInfo.UseShellExecute = false;

            
                _process.Start();
                _process.BeginOutputReadLine();
                Pid = _process.Id;
                Status = "Started";
                OnPropertyChanged(nameof(Status));
                Pid = _process.Id;
                OnPropertyChanged(nameof(Pid));
            }
            catch (Exception e)
            {
                
                InternalError += e.Message + Environment.NewLine;
                OnPropertyChanged(nameof(InternalError));

            }
        }

        private void ProcessOnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            ExeError += e.Data + Environment.NewLine;
            OnPropertyChanged(nameof(ExeError));
        }

        private void ProcessOnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            OutPut += e.Data + Environment.NewLine;
            OnPropertyChanged(nameof(OutPut));
           

        }

        public void PauseOutPut()
        {
            try
            {
                _process.CancelOutputRead();
            }
            catch (Exception e)
            {
                InternalError += e.Message;
                OnPropertyChanged(nameof(InternalError));
            }
        }

        public void BeginRead()
        {
            try
            {
                _process.BeginOutputReadLine();
            }
            catch (Exception e)
            {
                InternalError += e.Message;
                OnPropertyChanged(nameof(InternalError));
            }
        }

        public void Close()
        {
            try
            {
                _process.CancelOutputRead();
                _process.Close();
                Status = "Closed";
         
                OnPropertyChanged(nameof(Status));
            }
            catch (Exception e)
            {
                InternalError += e.Message;
                OnPropertyChanged(nameof(InternalError));
            }



        }
    


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName)); 
        }
    }
