using UfaSmartCity.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using UfaSmartCity.Data;

namespace UfaSmartCity
{
    public class ContextManager
    {
        private ApplicationDbContext db;

        public ContextManager()
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql("Server=192.168.10.10;Port=5432;Database=geraldDB;" +
                                                  "UserId=postgres;Password=$a!omonGrundyAnd$amoSV@L2019;");
            var db = new ApplicationDbContext(optionsBuilder.Options);
        }

        //public string GetUserRoleName(string userName)
        //{
        //    var context = db;
        //    var user = context.Users
        //                      .Any(u => string.Equals(u.UserName, userName))
        //             ? context.Users
        //                      .FirstOrDefault(u => string.Equals(u.UserName, userName))
        //             : null;
        //    if (user == null) return null;
        //    var role = context.Roles
        //                      .Any(r => r..Any(u => string.Equals(u.UserId, user.Id)))
        //                 ? context.Roles
        //                          .FirstOrDefault(r => r.Users.Any(u => string.Equals(u.UserId, user.Id)))
        //                 : null;
        //    if (role == null) return null;
        //    return role.Name;
        //}

        //public IdentityRole CheckRoleIsValid(string roleName)
        //{
        //    var context = ApplicationDbContext.Create();
        //    var role = context.Roles
        //                      .Any(r => string.Equals(r.Name, roleName))
        //             ? context.Roles
        //                      .FirstOrDefault(r => string.Equals(r.Name, roleName))
        //             : null;
        //    if (role == null)
        //    {
        //        context.Roles
        //               .Add(new IdentityRole(roleName));
        //        context.SaveChanges();
        //        if(roleName == "sterlitamak" && db.Stations
        //                                          .All(s=>!string.Equals(s.UserCity,roleName)))
        //        {
        //            var predictorManager = new SterlitamakPredictorManager();
        //            var stations = predictorManager.GetStations();
        //            var stationsM = new List<StationModel>();
        //            foreach (var item in stations)
        //            {
        //                var station = new StationModel(item,"s",roleName);
        //                stationsM.Add(station);
        //            }
        //            db.Stations.AddRange(stationsM);
        //            db.SaveChanges();
        //        }
        //    }
        //    return context.Roles
        //                  .FirstOrDefault(r => string.Equals(r.Name, roleName));
        //}

        public List<InformationTable> GetInformationTables()
        {
            return db.InformationTables.ToList();
        }

        public InformationTable GetInformationTable(string informationTableId)
        {
            return db.InformationTables
                     .Any(i => string.Equals(i.Id, informationTableId))
                  ? db.InformationTables
                      .FirstOrDefault(i => string.Equals(i.Id, informationTableId))
                  : null;
        }

        public string SetAccessCode(string stationId)
        {
            var station = db.Stations.Any(s => s.Id == stationId)
                        ? db.Stations.FirstOrDefault(s => s.Id == stationId)
                        : null;
            if (station == null) return null;
            var accessCode = station.AccessCode = Guid.NewGuid()
                                                      .ToString();
            db.SaveChanges();
            return accessCode;
        }

        //public List<StationModel> GetStations(string userName)
        //{
        //    var context_user_store = ApplicationDbContext.Create();
        //    var userId = context_user_store.Users
        //                                   .Any(a => string.Equals(a.UserName, userName))
        //               ? context_user_store.Users
        //                                   .FirstOrDefault(a => string.Equals(a.UserName, userName))
        //                                   .Id
        //               : null;
        //    if (userId == null) return db.Stations.ToList();
        //    var role = context_user_store.Roles
        //                                 .Any(r => r.Users.Any(u => string.Equals(u.UserId, userId)))
        //                 ? context_user_store.Roles
        //                                     .FirstOrDefault(r => r.Users.Any(u => string.Equals(u.UserId, userId)))
        //                 : null;
        //    if (role == null) return db.Stations.ToList();
        //    var city_key = role.Name;
        //    var stations = db.Stations
        //                     .Where(s => string.Equals(s.UserCity, city_key))
        //                     .ToList();
        //    return stations;                         
        //}

        public List<StationModel> GetStations()
        {
            return db.Stations.ToList();
        }

        public StationModel DeactivateInformationTable(string stationId)
        {
            var station = db.Stations.Any(s => string.Equals(s.Id, stationId))
                        ? db.Stations.FirstOrDefault(s => string.Equals(s.Id, stationId))
                        : null;
            if (station == null) return null;
            station.Active = false;
            db.SaveChanges();
            return station;
        }

        public StationModel GetStation(string stationId)
        {
            return db.Stations.Any(s => s.Id == stationId)
                   ? db.Stations.FirstOrDefault(s => s.Id == stationId)
                   : null;
        }

        public List<ModuleType> GetModuleTypes()
        {
            return db.ModuleTypes.ToList();
        }

        public List<Content> GetContents()
        {
            return db.Contents.ToList();
        }

        public Content GetContent(string contentId)
        {
            return db.Contents.Any(c => c.Id == contentId)
                 ? db.Contents.FirstOrDefault(c => c.Id == contentId)
                 : null;
        }

        public void ChangeContent(string contentId, Content newContent)
        {
            var content = db.Contents
                            .Any(c => string.Equals(c.Id, contentId))
                        ? db.Contents
                            .FirstOrDefault(c => string.Equals(c.Id, contentId))
                        : null;
            if (content == null) return;
            content.ContentType = newContent.ContentType;
            content.InnerContent = newContent.InnerContent;
            content.TimeOut = newContent.TimeOut;
            db.SaveChanges();
        }

        public void RemoveContent(string stationId, string contentId)
        {
            var station = db.Stations.Any(s => string.Equals(s.Id, stationId))
                        ? db.Stations.FirstOrDefault(s => string.Equals(s.Id, stationId))
                        : null;
            if (station?.InformationTable == null)
            {
                return;
            }
            var content = db.Contents.Any(c => string.Equals(c.Id, contentId))
                        ? db.Contents.FirstOrDefault(c => string.Equals(c.Id, contentId))
                        : null;
            if (content == null)
            {
                return;
            }
            station.InformationTable
                   .Contents
                   .Remove(content);
            db.SaveChanges();
        }

        public Content CreateContent(string stationId, Content newContent)
        {
            if (newContent == null) return null;
            var station = db.Stations
                            .Any(s => string.Equals(s.Id, stationId))
                        ? db.Stations
                            .FirstOrDefault(s => string.Equals(s.Id, stationId))
                        : null;
            if (station == null) return null;
            var informationTable = station.InformationTable;
            if (informationTable == null) return null;
            var content = new Content()
            {
                ContentType = newContent.ContentType,
                Id = newContent.Id ?? Guid.NewGuid()
                                          .ToString(),
                InnerContent = newContent.InnerContent ?? "",
                TimeOut = newContent.TimeOut
            };
            db.Contents.Add(content);
            db.SaveChanges();
            var newDbContent = db.Contents.Any(c => string.Equals(c.Id, content.Id))
                           ? db.Contents.FirstOrDefault(c => string.Equals(c.Id, content.Id))
                           : null;
            if (newDbContent == null) return null;
            informationTable.Contents.Add(newDbContent);
            db.SaveChanges();
            return newDbContent;
        }

        //public List<Content> GetContents(string informationTableId)
        //{
        //    return db.Contents
        //             .Where(c => string.Equals(c.Id, informationTableId))
        //             .ToList();
        //}

        public void SetModuleType(string informationTableId, string moduleTypeId)
        {
            var informationTable = db.InformationTables
                                     .Any(i => string.Equals(i.Id, informationTableId))
                                 ? db.InformationTables
                                     .FirstOrDefault(i => string.Equals(i.Id, informationTableId))
                                 : null;
            var moduleType = db.ModuleTypes
                               .Any(m => string.Equals(m.Id, moduleTypeId))
                           ? db.ModuleTypes
                               .FirstOrDefault(m => string.Equals(m.Id, moduleTypeId))
                           : null;
            if (informationTable == null || moduleType == null) return;
            informationTable.ModuleType = moduleType;
            db.SaveChanges();
        }

        public void ChangeInformationTable(string informationTableId, InformationTable newInformationTable)
        {
            var informationTable = db.InformationTables
                                     .Any(i => i.Id == informationTableId)
                                 ? db.InformationTables
                                     .FirstOrDefault(i => i.Id == informationTableId)
                                 : null;
            if (informationTable == null) return;
            informationTable.HeightWithModule = newInformationTable.HeightWithModule;
            informationTable.RowCount = newInformationTable.RowCount;
            informationTable.WidthWithModule = newInformationTable.WidthWithModule;
            db.SaveChanges();
        }

        public StationModel ActivateInformationTable(string stationId)
        {
            InformationTable defaultInformationTable = null;
            var defaultContent = new Content()
            {
                ContentType = ContentType.FORECAST,
                Id = Guid.NewGuid().ToString(),
                InnerContent = stationId,
                TimeOut = 15
            };
            var station = db.Stations
                            .Any(s => s.Id == stationId)
                        ? db.Stations
                            .FirstOrDefault(s => s.Id == stationId)
                        : null;
            var moduleType = db.ModuleTypes
                               .Any()
                           ? db.ModuleTypes
                               .FirstOrDefault()
                           : null;
            if (station == null || moduleType == null) return null;
            if (station.InformationTable == null)
            {
                defaultInformationTable = new InformationTable()
                {
                    Id = Guid.NewGuid()
                                 .ToString(),
                    HeightWithModule = 0,
                    WidthWithModule = 0,
                    RowCount = 0
                };
                defaultInformationTable.Contents
                                       .Add(defaultContent);
                db.InformationTables.Add(defaultInformationTable);
                db.SaveChanges();
                var informationTable = db.InformationTables
                    .Any(i => string.Equals(i.Id, defaultInformationTable.Id))
                    ? db.InformationTables
                        .FirstOrDefault(i => string.Equals(i.Id, defaultInformationTable.Id))
                    : null;
                if (informationTable == null) return null;
                informationTable.ModuleType = moduleType;
                station.InformationTable = informationTable;
            }
            //else
            //{
            //    station.Active = true;
            //    //defaultInformationTable = db.InformationTables
            //    //                            .Any(i => string.Equals(i.Id, station.InformationTable
            //    //                                                              .Id))
            //    //                        ? db.InformationTables
            //    //                            .FirstOrDefault(i => string.Equals(i.Id, station.InformationTable
            //    //                                                              .Id))
            //    //                        : null;
            //    //if (defaultInformationTable == null) return null;
            //    //defaultInformationTable.HeightWithModule = 0;
            //    //defaultInformationTable.RowCount = 0;
            //    //defaultInformationTable.WidthWithModule = 0;
            //    //defaultInformationTable.ModuleType = moduleType;
            //}
            //db.SaveChanges();
            station.Active = true;
            db.SaveChanges();
            return station;
        }
    }
}