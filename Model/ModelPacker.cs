using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklaModelPacker.ViewModel;

namespace TeklaModelPacker.Model
{
    public class ModelPacker
    {
        private List<string> report = new List<string>();
        private MainViewViewModel viewModel { get; set; }

        private readonly string date = DateTime.Now.ToString("MMM_dd_yy", (IFormatProvider) new CultureInfo("en-us")).ToUpper();
        
        private void zippingPackage(string localPath)
        {
            ZipFile.CreateFromDirectory(localPath, $"D:\\autozapis\\{Path.GetFileName(localPath)}.zip");
        }
        private void prepareReportAfterPacking(List<string> report,string modelName)
        {
            var reportPath = $"D:\\autozapis\\{date}_{modelName}_report_after_packing.txt";
            if(report.Count == 0) 
            {
                var text = "Everything have been prepared correctly. Send package to customer";
                File.WriteAllText(reportPath, text);
            }
            else
            {
                File.WriteAllLines(reportPath, report);
            }

            Process.Start("notepad.exe",reportPath);
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
        public ModelPacker(MainViewViewModel ViewModel)
        {
            viewModel = ViewModel;
        }
        public async Task createPackage()
        {
            List<Task> tasks = new List<Task>();
            var directoires = Directory.EnumerateDirectories(viewModel.SelectedModelPath);
            var files = Directory.GetFiles(viewModel.SelectedModelPath);
            var newModelName = date + "_" + Path.GetFileName(viewModel.SelectedModelPath) + "_TEKLA MODEL";
            var localPath = Path.Combine("d:\\autozapis\\",newModelName);
            Directory.CreateDirectory(localPath);
            List<Task> tasks1 = new List<Task>();

            if (viewModel.DrawingsFiles)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var paths = directoires.Select((path) => new { path = path }).Where(x => x.path.Contains("drawings"));
                    CopyElements(paths.First().path, localPath);
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
                        CopyElements(paths.First().path, localPath);
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
                        File.Copy(file, Path.Combine(localPath, Path.GetFileName(file)), true);
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
                        File.Copy(file, Path.Combine(localPath, Path.GetFileName(file)), true);
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
                        File.Copy(file, Path.Combine(localPath, Path.GetFileName(file)), true);
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
                        File.Copy(file, Path.Combine(localPath, Path.GetFileName(file)), true);
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
                        File.Copy(file, Path.Combine(localPath, Path.GetFileName(file)), true);
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
                                if (Path.GetFileName(file).Contains(Path.GetFileName(viewModel.SelectedModelPath)))
                                {
                                    var tmpName = Path.GetFileName(viewModel.SelectedModelPath);
                                    tmpName = date + "_" + tmpName+"_TEKLA_MODEL"+data;
                                    Path.Combine(localPath, tmpName);
                                    File.Copy(file, Path.Combine(localPath, tmpName));
                                }
                                else
                                {
                                    File.Copy(file, Path.Combine(localPath, Path.GetFileName(file)));
                                }
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

            zippingPackage(localPath);
            prepareReportAfterPacking(report,Path.GetFileName(viewModel.SelectedModelPath));
            Directory.Delete(localPath, true);
        }
    }
}
