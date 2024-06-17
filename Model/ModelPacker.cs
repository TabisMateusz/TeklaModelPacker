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
        private void CopyElements(string path, string packagePath)
        {
            Directory.CreateDirectory(packagePath + $"\\{Path.GetFileName(path)}");
            foreach (var file in Directory.GetFiles(path))
            {
                try
                {
                    File.Copy(file, Path.Combine(packagePath + $"\\{Path.GetFileName(path)}", Path.GetFileName(file)), true);
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

        public async void CreatePackage(MainViewViewModel viewModel)
        {
            List<Task> tasks = new List<Task>();
            var directoires = Directory.EnumerateDirectories(viewModel.SelectedModelPath);
            var files = Directory.GetFiles(viewModel.SelectedModelPath);
            Directory.CreateDirectory(viewModel.SelectedModelPath);

            List<Task> tasks1 = new List<Task>();

            if (viewModel.DrawingsFiles)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var paths = directoires.Select((path) => new { path = path }).Where(x => x.path.Contains("drawings"));
                    CopyElements(paths.First().path, viewModel.SelectedModelPath);
                }));
            }
            if (viewModel.AttributesFolder)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    string[] attributes = { "AI_imp", "attributes" };
                    foreach (var element in attributes)
                    {
                        var paths = directoires.Select((path) => new { path = path }).Where(x => x.path.Contains(element));
                        CopyElements(paths.First().path, viewModel.SelectedModelPath);
                    }
                }));
            }
            if (viewModel.ObjectsInp)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var file = files.Where(x => x.Contains("objects.inp")).FirstOrDefault();
                    File.Copy(file, Path.Combine(viewModel.SelectedModelPath, Path.GetFileName(file)), true);
                }));
            }
            if (viewModel.ProfileCatalog)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var file = files.Where(x => x.Contains("profdb.bin")).FirstOrDefault();
                    File.Copy(file, Path.Combine(viewModel.SelectedModelPath, Path.GetFileName(file)), true);
                }));

            }
            if (viewModel.MaterialCatalog)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var file = files.Where(x => x.Contains("matdb.bin")).FirstOrDefault();
                    File.Copy(file, Path.Combine(viewModel.SelectedModelPath, Path.GetFileName(file)), true);
                }));

            }
            if (viewModel.BoltCatalog)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var file = files.Where(x => x.Contains("screwdb.db")).FirstOrDefault();
                    File.Copy(file, Path.Combine(viewModel.SelectedModelPath, Path.GetFileName(file)), true);
                }));
            }
            if (viewModel.BoltAssemblyCatalog)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var file = files.Where(x => x.Contains("assdb.db")).FirstOrDefault();
                    File.Copy(file, Path.Combine(viewModel.SelectedModelPath, Path.GetFileName(file)), true);
                }));

            }
            tasks.Add(Task.Factory.StartNew(() =>
            {
                string[] extensions = { ".db1", ".db2",".tpl" };
                foreach (var data in extensions)
                {
                    foreach (var file in files.Where(x => x.EndsWith(data)))
                    {
                        File.Copy(file, Path.Combine(viewModel.SelectedModelPath, Path.GetFileName(file)), true);

                    }
                }
            }));

            await Task.WhenAll(tasks);
        }
    }
}
