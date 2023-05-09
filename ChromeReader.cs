using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LoginData
{
    internal class ChromeReader : IPassReader
    {
        public string BrowserName { get { return "Chrome"; } }


        //Chemin vers le fichier de Login pour Google Chrome
        //private const string LOGIN_DATA_PATH = "\\..\\Local\\Google\\Chrome\\User Data\\Default\\Login Data";
        private const string LOGIN_DATA_PATH = "\\..\\Local\\Google\\Chrome\\User Data\\Profile 2\\Login Data";
        string chemin = @"C:\Users\Ragot_Prod\AppData\Local\Google\Chrome\User Data\Profile 2\Login Data";

        //Local\\Google\\Chrome\\User Data\\Profile 2\\Login Data"
        //collection des login enregistrés
        public IEnumerable<CredentialModel> ReadPasswords()
        {
            var result = new List<CredentialModel>();
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);// APPDATA
            var pathDB = Path.GetFullPath(appdata + LOGIN_DATA_PATH);

            //string fileName = "Login Data";
            string targetPath = AppDomain.CurrentDomain.BaseDirectory;

            string cheminfin = targetPath + @"\LoginsChrome\";//"";
            //string destFile = System.IO.Path.Combine(cheminfin, fileName);

            if (File.Exists(cheminfin))
            {
                //MessageBox.Show("E1");
                //chek si le fichier db existe deja
                if (!File.Exists(cheminfin + Path.GetFileName("\\Login Data.db")))
                {
                    //MessageBox.Show("E1.1");
                    // rename le fichier (on fait juste une copie du fichier avec le nom qu'on veut)
                    System.IO.File.Copy(pathDB, cheminfin + Path.GetFileName("\\Login Data.db"), true);
                    // efface l'ancien fichier
                    //System.IO.File.Delete(destFile);
                }

            }
            else
            {
                //copy le fichier
                System.IO.File.Copy(pathDB, cheminfin + Path.GetFileName(pathDB), true);
                // puis rename le fichier 
                //MessageBox.Show("sinon");
                //chek si le fichier db existe deja
                if (!File.Exists(cheminfin + Path.GetFileName("\\Login Data.db")))
                {
                    //MessageBox.Show("sinon.1");
                    System.IO.File.Copy(pathDB, cheminfin + Path.GetFileName("\\Login Data.db"), true);
                    //System.IO.File.Delete(destFile); // delete
                }

            }

            //le fichier copie devient maintenant  +".db"
            string sourceFile = cheminfin + Path.GetFileName("\\Login Data.db");

            if (File.Exists(sourceFile))
            {
                Console.WriteLine("First ok");
                using (var conn = new SQLiteConnection("Data Source=" + sourceFile + ";"))
                {
                    conn.Open();

                    string CommandText = "SELECT action_url, username_value, password_value FROM logins";

                    var cmd = new SQLiteCommand(CommandText, conn);

                    SQLiteDataReader rdr = cmd.ExecuteReader();
                    var key = GCDecryptor.GetKey();

                    while (rdr.Read())
                    {

                        byte[] nonce, ciphertextTag;
                        var encryptedData = GetBytes(rdr, 2);
                        GCDecryptor.Prepare(encryptedData, out nonce, out ciphertextTag);
                        var pass = GCDecryptor.Decrypt(ciphertextTag, key, nonce);

                        result.Add(new CredentialModel()
                        {
                            Url = rdr.GetString(0),
                            Username = rdr.GetString(1),
                            Password = pass
                        });

                    }


                    conn.Close();
                }

            }
            else
            {
                Console.WriteLine("Can not find chrome logins file");
                throw new FileNotFoundException("Can not find chrome logins file");
            }
            return result;
        }


        private Stream CopyFileInMemory(string path)
        {
            MemoryStream inMemoryCopy = new MemoryStream();

            using (FileStream fs = File.OpenRead(path))
            {
                fs.CopyTo(inMemoryCopy);
            }

            return inMemoryCopy;
        }
        private byte[] GetBytes(SQLiteDataReader reader, int columnIndex)
        {
            const int CHUNK_SIZE = 2 * 1024;
            byte[] buffer = new byte[CHUNK_SIZE];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(columnIndex, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }

        /****************ici le constructeur *******/
        public ChromeReader()
        {
            //MessageBox.Show("je suis le navigateur Chrome + " + LOGIN_DATA_PATH);
        }


    }
}
