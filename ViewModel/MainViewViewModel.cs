using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using TeklaModelPacker.Model;

namespace TeklaModelPacker.ViewModel
{
    public class MainViewViewModel : BaseViewModel
    {
        #region Properties

        private string _packageName;

        public string PackageName
        {
            get 
            { 
                return _packageName; 
            }
            set
            { 
                if(_packageName != value) 
                {
                    _packageName = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _drawingsFiles;

        public bool DrawingsFiles
        {
            get 
            {
                return _drawingsFiles; 
            }
            set 
            { 
                if(_drawingsFiles != value) 
                {
                    _drawingsFiles = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _attributesFolder;

        public bool AttributesFolder
        {
            get { return _attributesFolder; }
            set
            {
                if (_attributesFolder != value)
                {
                    _attributesFolder = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _objectsInp;

        public bool ObjectsInp
        {
            get { return _objectsInp; }
            set 
            { 
                if (_attributesFolder != value)
                {
                    _objectsInp = value;
                    OnPropertyChanged();
                }

            }
        }

        private bool _profileCatalog;

        public bool ProfileCatalog
        {
            get { return _profileCatalog; }
            set 
            {
                if (_profileCatalog != value)
                {
                    _profileCatalog = value;
                    OnPropertyChanged();
                }

            }
        }

        private bool _materialCatalog;

        public bool MaterialCatalog
        {
            get { return _materialCatalog; }
            set 
            { 
                if (_materialCatalog != value)
                {
                    _materialCatalog = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _boltCatalog;

        public bool BoltCatalog
        {
            get { return _boltCatalog; }
            set 
            {
                if (_boltCatalog != value)
                {
                    _boltCatalog = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _boltAssemblyCatalog;

        public bool BoltAssemblyCatalog
        {
            get { return _boltAssemblyCatalog; }
            set 
            {
                if (_boltAssemblyCatalog != value)
                {
                    _boltAssemblyCatalog = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SelectedModelPath { get; set; }

        #endregion


        public ICommand BrowseFoldersCommand { get; set; }
        public ICommand CreateZipFileCommand { get; set; }
        public MainViewViewModel()
        {
            BrowseFoldersCommand = new RelayCommand(findModel);
            CreateZipFileCommand = new RelayCommand(createZip);
            ProfileCatalog = true;
            MaterialCatalog = true;
            BoltCatalog = true;
            BoltAssemblyCatalog = true;
        }

        private void findModel(object parameter)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath))
                {
                    PackageName = new DirectoryInfo(dialog.SelectedPath).Name;
                    //SelectedModelPath = dialog.SelectedPath;
                }
            }
            
        }
        private void createZip(object parameter) 
        { 
            ModelPacker modelPacker = new ModelPacker();
            SelectedModelPath = "D:\\Models\\TEKLA 2022\\PROGRAMING_TESTS";

            modelPacker.CreatePackage(this);
        
        
        }
    }
}
