using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklaModelPacker.ViewModel;

namespace TeklaModelPacker.Model
{
    public class ModelPacker
    {
        private List<string> report = new List<string>();

        private string date = DateTime.Now.ToString("MMM_dd_yy", (IFormatProvider) new CultureInfo("en-us")).ToUpper();

        private void prepareReportAfterPacking(List<string> report,string modelName)
        {
            var reportPath = $"C:\\Users\\matau\\OneDrive\\Pulpit\\{date}_{modelName}_report_after_packing.txt";
            if(report.Count == 0) 
            {
                var text = "Everything was prepared well. Sent package to client";
                File.WriteAllText(reportPath, text);
                File.Open(reportPath, FileMode.Open);
            }
            else
            {
                File.WriteAllLines(reportPath, report);
                File.Open(reportPath, FileMode.Open);
            }
        }
        private void CopyElements(string path, string packagePath)
        {
            Directory.CreateDirectory(packagePath + $"\\{Path.GetFileName(path)}");
            foreach (var file in Directory.GetFiles(path))
            {
                try
                {
                    File.Copy(file, Path.Combine(packagePath + $"\\{Path.GetFileName(path)}", Path.GetFileName(file)), true);
                }
                catch (Exception ex)
                {
                    report.Add(ex.Message);
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
                    try
                    {
                        var file = files.Where(x => x.Contains("objects.inp")).FirstOrDefault();
                        File.Copy(file, Path.Combine(viewModel.SelectedModelPath, Path.GetFileName(file)), true);
                    }
                    catch (Exception ex) 
                    {
                        report.Add(ex.Message);
                    }
                }));
            }
            if (viewModel.ProfileCatalog)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var file = files.Where(x => x.Contains("profdb.bin")).FirstOrDefault();
                        File.Copy(file, Path.Combine(viewModel.SelectedModelPath, Path.GetFileName(file)), true);
                    }
                    catch (Exception ex) 
                    {
                        report.Add(ex.Message);
                    }
                }));

            }
            if (viewModel.MaterialCatalog)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var file = files.Where(x => x.Contains("matdb.bin")).FirstOrDefault();
                        File.Copy(file, Path.Combine(viewModel.SelectedModelPath, Path.GetFileName(file)), true);
                    }
                    catch (Exception ex)
                    {
                        report.Add(ex.Message); 
                    }
                }));

            }
            if (viewModel.BoltCatalog)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var file = files.Where(x => x.Contains("screwdb.db")).FirstOrDefault();
                        File.Copy(file, Path.Combine(viewModel.SelectedModelPath, Path.GetFileName(file)), true);
                    }
                    catch (Exception ex) 
                    {
                        report.Add(ex.Message);
                    }
                }));
            }
            if (viewModel.BoltAssemblyCatalog)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    try
                    {
                        var file = files.Where(x => x.Contains("assdb.db")).FirstOrDefault();
                        File.Copy(file, Path.Combine(viewModel.SelectedModelPath, Path.GetFileName(file)), true);
                    }
                    catch (Exception ex) 
                    {
                        report.Add(ex.Message);
                    }
                }));

            }
            tasks.Add(Task.Factory.StartNew(() =>
            {
                string[] extensions = { ".db1", ".db2",".tpl",".sym" };
                try
                {
                    foreach (var data in extensions)
                    {
                        foreach (var file in files.Where(x => x.EndsWith(data)))
                        {
                            try
                            {
                                File.Copy(file, Path.Combine(viewModel.SelectedModelPath, Path.GetFileName(file)), true);
                            }
                            catch (Exception ex)
                            {
                                report.Add(ex.Message);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    report.Add(ex.Message);
                }
                
            }));

            await Task.WhenAll(tasks);

            prepareReportAfterPacking(report,Path.GetFileName(viewModel.SelectedModelPath));


        }
    }
}
