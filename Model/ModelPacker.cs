using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklaModelPacker.ViewModel;

namespace TeklaModelPacker.Model
{
    public class ModelPacker
    {
        private void CopyElements(string path,string packagePath)
        {
            Directory.CreateDirectory(packagePath+$"\\{Path.GetFileName(path)}");
            foreach (var file in Directory.GetFiles(path))
            {
                try
                {
                    File.Copy(file, Path.Combine(packagePath+ $"\\{Path.GetFileName(path)}",Path.GetFileName(file)) , true);
                }
                catch 
                { 
                
                }
            }
            var subDirectories = Directory.GetDirectories(path);
            foreach (var directory in subDirectories)
            {
                CopyElements(directory, packagePath + $"\\{Path.GetFileName(path)}");
            }
        }

        public void CreatePackage(MainViewViewModel viewModel)
        {
            var packagePath = $"D:\\AUTOZAPIS\\TEST";
            var directoires = Directory.EnumerateDirectories(viewModel.SelectedModelPath);
            var files = Directory.GetFiles(viewModel.SelectedModelPath);
            Directory.CreateDirectory(packagePath);

            if (viewModel.DrawingsFiles)
            {
                var paths = directoires.Select((path) => new { path = path }).Where(x => x.path.Contains("drawings"));
                
                CopyElements(paths.First().path, packagePath);
            }
            if (viewModel.AttributesFolder)
            {
                string[] attributes = { "AI_imp", "attributes" };
                foreach (var element in attributes)
                {
                    var paths = directoires.Select((path) => new { path = path }).Where(x => x.path.Contains(element));

                    CopyElements(paths.First().path, packagePath);
                }
                
            }
            if (viewModel.ObjectsInp)
            {
                var objectInp = files.Where(x => x.Contains("objects.inp")).FirstOrDefault();
                File.Copy(objectInp, Path.Combine(packagePath, Path.GetFileName(objectInp)), true);

            }
            if (viewModel.ProfileCatalog)
            {
                var objectInp = files.Where(x => x.Contains("objects.inp")).FirstOrDefault();
                File.Copy(objectInp, Path.Combine(packagePath, Path.GetFileName(objectInp)), true);

            }
            if (viewModel.MaterialCatalog)
            {
                var objectInp = files.Where(x => x.Contains("objects.inp")).FirstOrDefault();
                File.Copy(objectInp, Path.Combine(packagePath, Path.GetFileName(objectInp)), true);

            }
            if (viewModel.BoltCatalog)
            {
                var objectInp = files.Where(x => x.Contains("objects.inp")).FirstOrDefault();
                File.Copy(objectInp, Path.Combine(packagePath, Path.GetFileName(objectInp)), true);

            }
            if (viewModel.BoltAssemblyCatalog)
            {
                var objectInp = files.Where(x => x.Contains("objects.inp")).FirstOrDefault();
                File.Copy(objectInp, Path.Combine(packagePath, Path.GetFileName(objectInp)), true);

            }
            

        }
    }
}
