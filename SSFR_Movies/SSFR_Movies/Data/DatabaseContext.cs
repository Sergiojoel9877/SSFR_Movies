//using SSFR_Movies.Models;
////using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text;
//using Xamarin.Forms;
//using Xamarin.Forms.Internals;

//namespace SSFR_Movies.Data
//{
//    /// <summary>
//    /// The DatabaseContext for the Database.
//    /// </summary>
//    /// <typeparam name="T">Any object that inherit from BaseEntity</typeparam>
//    [Preserve(AllMembers = true)]
//    public class DatabaseContext<T> : DbContext where T : class
//    {
//        public DbSet<T> Entity { get; set; }

//        string DbName = "SSFR_Movies.db3";
        
//        public DatabaseContext()
//        {
//            Database.EnsureCreated();
//        }

//        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
//        {
//            string path = "";

//            switch (Device.RuntimePlatform)
//            {
//                case Device.iOS:

//                    SQLitePCL.Batteries_V2.Init();

//                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "..", "Library", DbName);

//                    dbContextOptionsBuilder.UseSqlite($"Filename={path}");

//                    break;
//                case Device.Android:

//                    path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), DbName);

//                    dbContextOptionsBuilder.UseSqlite($"Filename={path}");

//                    break;

//                default:
//                    throw new NotImplementedException("Platform not supported");
//            }
//        }
//    }
//}
