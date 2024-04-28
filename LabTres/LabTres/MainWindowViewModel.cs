using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace LabTres
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private AssemblyInfo _assembly;

        public AssemblyInfo Assembly
        {
            get { return _assembly; }
            set
            {
                _assembly = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            // Specify the path to your .NET assembly here
            string assemblyPath = "X:/1Third_Year/MPP/ModernProgrammingPlatforms_Labs/LabTres/LabTres/bin/Debug/net8.0-windows/LabTres.dll";
            // string assemblyPath = "X:/1Third_Year/GraduationProject_Bimqa/Bimqa/bin/Debug/OpenTK.dll";
            Assembly = new AssemblyInfo(assemblyPath);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}