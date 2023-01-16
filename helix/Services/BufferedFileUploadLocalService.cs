using FitsLibrary;
using helix.Data;
using helix.Models;
using helix.Services.interfaces;
using ImageMagick;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Claims;

namespace helix.Services
{
    public class BufferedFileUploadLocalService : IBufferedFileUploadService
    {
        private readonly ApplicationDbContext _context;
        string path = "";
        public BufferedFileUploadLocalService(ApplicationDbContext context)
        {
            _context = context;
            path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "UploadedFiles"));
        }
        public async Task<List<KeyValuePair<int, bool>>> UploadFile(string Id, IFormFile[] files)
        {
            var _path = Path.Combine(path, Id);
            List<KeyValuePair<int, bool>> result = new List<KeyValuePair<int, bool>>();

            var _extensions = new string[] { ".jpg", ".png", ".fits" };


            var filesList = files.ToList();
            foreach (var file in filesList)
            {
                try
                {
                    if (file.Length > 0)
                    {
                        #region CheckExtention
                        var extension = Path.GetExtension(file.FileName);
                        if (!_extensions.Contains(extension.ToLower()))
                        {
                            throw new Exception("File Not Valid!");
                        }
                        #endregion

                        if (!Directory.Exists(_path))
                        {
                            Directory.CreateDirectory(_path);
                        }
                        using (var fileStream = new FileStream(Path.Combine(_path, file.FileName), FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }
                        result.Add(new KeyValuePair<int, bool>(filesList.IndexOf(file), true));
                    }
                    else
                    {
                        result.Add(new KeyValuePair<int, bool>(filesList.IndexOf(file), false));
                    }
                }
                catch (Exception ex)
                {
                    result.Add(new KeyValuePair<int, bool>(filesList.IndexOf(file), false));
                    //throw new Exception("File Copy Failed", ex);
                }
            }
            return result;
        }

        public async Task<List<KeyValuePair<int, ObservationSubmission>>> UploadFileFits(IFormFile[] files)
        {

            List<KeyValuePair<int, ObservationSubmission>> result = new List<KeyValuePair<int, ObservationSubmission>>();

            var _extensions = new string[] { ".fits" };

            var filesList = files.ToList();
            foreach (var file in filesList)
            {
                try
                {
                    var Id = Guid.NewGuid();
                    var _path = Path.Combine(path, Id.ToString());
                    if (file.Length > 0)
                    {
                        #region CheckExtention
                        var extension = Path.GetExtension(file.FileName);
                        if (!_extensions.Contains(extension.ToLower()))
                        {
                            throw new Exception("File Not Valid!");
                        }
                        #endregion

                        if (!Directory.Exists(_path))
                        {
                            Directory.CreateDirectory(_path);
                        }

                        var path_temp = Path.Combine(_path, file.FileName);

                        using (var fileStream = new FileStream(path_temp, FileMode.Create))
                        {
                            await file.CopyToAsync(fileStream);
                        }

                        var thumb = Path.Combine(_path, "thumb");
                        if (!Directory.Exists(thumb))
                        {
                            Directory.CreateDirectory(thumb);
                        }
                        var thumb_temp = Path.Combine(thumb, file.FileName);


                        using (var image = new MagickImage(path_temp))
                        {
                            image.LinearStretch(new Percentage(50), new Percentage(80));
                            image.ContrastStretch(new Percentage(99), new Percentage(100));
                            image.Write(thumb_temp.Replace(".fits",".jpeg"));
                        }


                        var reader = new FitsDocumentReader();
                        var fitsFile = await reader.ReadAsync(path_temp);

                        var obs = new ObservationSubmission
                        {
                            Id = Id,
                            Type=Convert.ToString(fitsFile.Header["FRAME"]).Trim(),
                        };

                        var FILTER = fitsFile.Header["FILTER"].ToString().Trim();
                        var TELESCOP = fitsFile.Header["TELESCOP"].ToString().Trim();
                        var OBJECT = fitsFile.Header["OBJECT"].ToString().Trim();

                        var RA = fitsFile.Header["RA"].ToString().Trim();
                        var DEC = fitsFile.Header["DEC"].ToString().Trim();


                        // search filter
                        var _filters = _context.Filters.Where(e => e.Name==FILTER).FirstOrDefault();
                        if (_filters==null)
                        {
                            Filter _filter = new Filter()
                            {
                                Id=0,
                                Name=FILTER,
                            };
                            _context.Filters.Add(_filter);
                            _context.SaveChanges();
                            _filters=_filter;
                        }

                        obs._Filter = _filters;

                        var _telescops = _context.Telescopes.Where(e => e.Name==TELESCOP).FirstOrDefault();
                        if (_telescops==null)
                        {
                            Telescope _telescope = new Telescope()
                            {
                                Id=0,
                                Name=TELESCOP,
                            };
                            _context.Telescopes.Add(_telescope);
                            _context.SaveChanges();
                            _telescops=_telescope;
                        }

                        obs._Telescope=_telescops;

                        var _objects = _context.SObjects.Where(e => e.Name==OBJECT).FirstOrDefault();
                        if (_objects==null)
                        {
                            SObject _object = new SObject()
                            {
                                Id=0,
                                Name =OBJECT,
                                RA=RA,
                                DEC=DEC,
                            };

                            _context.SObjects.Add(_object);
                            _context.SaveChanges();
                            _objects=_object;
                        }

                        obs._SObject=_objects;


                        if (fitsFile.Header["Detector"]==null)
                        {
                            var _detectors = _context.Detectors.Where(e => e.Name=="Unknown").FirstOrDefault();
                            if (_detectors==null)
                            {
                                var _detector = new Detector()
                                {
                                    Name="Unknown",
                                };
                                _context.Detectors.Add(_detector);
                                _context.SaveChanges();
                                _detectors=_detector;
                            }
                            obs._Detector=_detectors;
                        }
                        else
                        {
                            var Detector = fitsFile.Header["Detector"].ToString().Trim();

                            var _detectors = _context.Detectors.Where(e => e.Name==Detector).FirstOrDefault();
                            if (_detectors==null)
                            {
                                var _detector = new Detector()
                                {
                                    Name=Detector,
                                };
                                _context.Detectors.Add(_detector);
                                _context.SaveChanges();
                                _detectors=_detector;
                            }
                            obs._Detector=_detectors;
                        }

                        obs.Status = "False";
                        obs.Name = "Unknown";
                        obs.DateTime=DateTime.Parse(fitsFile.Header["DATE-OBS"].ToString());


                        result.Add(new KeyValuePair<int, ObservationSubmission>(filesList.IndexOf(file), obs));
                    }
                    else
                    {
                        result.Add(new KeyValuePair<int, ObservationSubmission>(filesList.IndexOf(file), null));
                    }
                }
                catch (Exception ex)
                {
                    result.Add(new KeyValuePair<int, ObservationSubmission>(filesList.IndexOf(file), null));
                    //throw new Exception("File Copy Failed", ex);
                }
            }
            return result;
        }

        public async Task<bool> DeleteFile(string Id, string fileName)
        {
            var _path = Path.Combine(path, Id);
            if (File.Exists(Path.Combine(_path, fileName)))
            {
                File.Delete(Path.Combine(_path, fileName));
                await Task.Delay(millisecondsDelay: 200);
                return true;
            }
            else
            {
                await Task.Delay(millisecondsDelay: 200);
                return false;
            }
        }
        public async Task<List<string>> GetFiles(string Id)
        {
            var _path = Path.Combine(path, Id);
            var fileEntries = Directory.GetFiles(_path).Select(e => e.Replace(_path, "").Replace("\\", "")).ToList();
            await Task.Delay(millisecondsDelay: 200);
            return fileEntries;
        }

        public async Task<string> GetFITSAsImage(string Id, string Name)
        {
            var _path = Path.Combine(path, Id);
            var path_temp = Path.Combine(_path, Name);

            var reader = new FitsDocumentReader();
            var fitsFile = await reader.ReadAsync(path_temp);


            var img = ConvertToImage(path_temp);
            return ImageToBase64(img, ImageFormat.Jpeg);
        }

        public static Bitmap ByteToImage(byte[] blob)
        {
            using (MemoryStream mStream = new MemoryStream())
            {
                mStream.Write(blob, 0, blob.Length);
                mStream.Seek(0, SeekOrigin.Begin);

                Bitmap bm = new Bitmap(mStream);
                return bm;
            }
        }
        private string ImageToBase64(System.Drawing.Image image, System.Drawing.Imaging.ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                // Convert Image to byte[]
                image.Save(ms, format);
                byte[] imageBytes = ms.ToArray();
                // Convert byte[] to Base64 String
                string base64String = Convert.ToBase64String(imageBytes);
                return base64String;
            }
        }
        private System.Drawing.Image ConvertToImage(string pathfitsfile)
        {
            int totalmax = 0;
            int totalmin = 0;
            int wavelength = 0;


            System.Drawing.Image result = null;
            //http://fits.gsfc.nasa.gov/fits_libraries.html#CSharpFITS
            //Read the fits file
            nom.tam.fits.Fits fits = new nom.tam.fits.Fits(pathfitsfile);
            nom.tam.fits.BasicHDU basichdu;
            do
            {
                basichdu = fits.ReadHDU();
                if (basichdu != null)
                {
                    basichdu.Info();
                }                                            //end if
            }                                                //end do
            while (basichdu != null);

            //loop through the Header Data Units
            for (int i = 0; i < fits.NumberOfHDUs; i++)
            {
                basichdu = fits.GetHDU(i);
                //this retreives the current bandwidth
                string card = basichdu.Header.GetCard(20);
                wavelength = System.Convert.ToInt32(card.Replace("WAVELNTH=", "").Trim());
                if (basichdu.GetType().FullName == "nom.tam.fits.ImageHDU")
                {
                    try
                    {
                        nom.tam.fits.ImageHDU imghdu = (nom.tam.fits.ImageHDU)basichdu;
                        //get out the pixel array and the dimensions
                        Array[] array = (Array[])imghdu.Data.DataArray;
                        int x = imghdu.Axes[0];
                        int y = imghdu.Axes[1];
                        result = new System.Drawing.Bitmap(x, y);
                        System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(result);
                        int idx_x = 0;
                        int idx_y = 0;
                        //get the real min/max values.
                        //We need it to get the values between 0 - 255.
                        for (int j = array.Length-1; j >= 0; j--)
                        {
                            int[] row = (int[])array[j];
                            for (int k = 0; k < row.Length; k++)
                            {
                                totalmax = (row[k] > totalmax ? row[k] : totalmax);
                                totalmin = (row[k] < totalmin ? row[k] : totalmin);
                            }                                //end for
                        }

                        //offset the values so everything is positive
                        int offset = (int)(totalmin < 0.0 ? (-1)*totalmin : 0.0);
                        //bottom is top, top is bottom.
                        for (int j = array.Length-1; j >= 0; j--)
                        {
                            //pixels are NOT RGB values so here we convert it to an RGB value.
                            idx_x = 0;
                            idx_y++;
                            int[] row = (int[])array[j];
                            for (int k = 0; k < row.Length; k++)
                            {
                                double val = row[k];
                                //This was trial and error.
                                switch (wavelength)
                                {
                                    case 335:
                                    case 94:
                                    case 131:
                                        break;
                                    case 304:
                                        val /= 3;
                                        break;
                                    case 171:
                                    case 193:
                                    case 211:
                                    case 1600:
                                        val /= 32;
                                        break;
                                    case 1700:
                                        val /= 128;
                                        break;
                                    case 4500:
                                        val /= 1024;
                                        break;
                                    default:
                                        break;
                                }                            //end switch
                                //make sure all pixels are in range 0 - 255
                                val = (val > 255.0 ? val = 255.0 : val);
                                val = (val < 0.0 ? val = 0.0 : val);
                                System.Drawing.Color c = System.Drawing.Color.FromArgb(2013265920 + (int)val);
                                //call this function to color the image depending on wavelength
                                c = WeighRGBValue(System.Drawing.Color.FromArgb(c.A, c.B, c.B, c.B), wavelength);
                                ((System.Drawing.Bitmap)result).SetPixel(idx_x, idx_y, c);
                                idx_x++;
                            }

                        }

                    }                                        //end try
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                    }                                        //end catch
                }                                            //end if
            }                                                //end for
            return result;
        }
        private System.Drawing.Color WeighRGBValue(System.Drawing.Color color, int wavelength)
        {
            //Tried to get a similar color to the original site.
            System.Drawing.Color c;
            int alpha, red, green, blue;
            red = green = blue = 255;
            alpha = color.A;
            switch (wavelength)
            {
                case 94:    //green : 00FF00
                    red = 0;
                    green = color.G;
                    blue = 0;
                    break;
                case 131:    //teal : 008080
                    red = 0;
                    green = color.G;
                    blue = color.B;
                    break;
                case 304:    //red : FF0000
                    red = color.R;
                    green = color.G / 5;
                    blue = color.B / 5;
                    break;
                case 335:    //blue : 0000FF
                    red = (color.R / 5);
                    green = (color.G / 5);
                    blue = color.B;
                    break;
                case 171:    //gold : FFD700
                    red = color.R;
                    green = (int)((double)color.G / 255.0 * 215.0);
                    blue = 0;
                    break;
                case 193:    //copper : B87333
                    red = color.R;                                    //184 mapped to 255
                    green = (int)((double)color.G / 255.0 * 115.0);    //115 mapped to 255
                    blue = (int)((double)color.B / 255.0 * 51.0);    //50 mapped to 255
                    break;
                case 211:    //purple : 800080
                    red = color.R;
                    green = 0;
                    blue = color.B;
                    break;
                case 1600:    //ocher : BBBB00
                    red = (int)((double)color.R / 255.0 * 187.0);    //BB mapped to 255
                    green = (int)((double)color.G / 255.0 * 187.0);    //BB mapped to 255
                    blue = 0;
                    break;
                case 1700:    //pink : FFC0CB
                    red = color.R;
                    green = (int)((double)color.G / 255.0 * 192.0);
                    blue = (int)((double)color.B / 255.0 * 203.0);
                    break;
                case 4500:    //silver : C0C0C0
                    red = color.R;
                    green = color.G;
                    blue = color.B;
                    break;
                default:
                    red = 0;
                    green = 0;
                    blue = 0;
                    break;
            }                            //end switch
            c = System.Drawing.Color.FromArgb(alpha, red, green, blue);
            return c;
        }
    }
}
