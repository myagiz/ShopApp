﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ShopApp.WebUI.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ShopAppEntities : DbContext
    {
        public ShopAppEntities()
            : base("name=ShopAppEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<tblAyarlar> tblAyarlar { get; set; }
        public virtual DbSet<tblCesitler> tblCesitler { get; set; }
        public virtual DbSet<tblIletisim> tblIletisim { get; set; }
        public virtual DbSet<tblKategoriler> tblKategoriler { get; set; }
        public virtual DbSet<tblLog> tblLog { get; set; }
        public virtual DbSet<tblMarkalar> tblMarkalar { get; set; }
        public virtual DbSet<tblMenuler> tblMenuler { get; set; }
        public virtual DbSet<tblMusteriler> tblMusteriler { get; set; }
        public virtual DbSet<tblSiparisKaydi> tblSiparisKaydi { get; set; }
        public virtual DbSet<tblSlider> tblSlider { get; set; }
        public virtual DbSet<tblStokYonetimi> tblStokYonetimi { get; set; }
        public virtual DbSet<tblUrunler> tblUrunler { get; set; }
        public virtual DbSet<tblYetki> tblYetki { get; set; }
        public virtual DbSet<tblYonetici> tblYonetici { get; set; }
    }
}
