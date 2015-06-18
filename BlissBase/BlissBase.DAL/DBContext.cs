// DAL "Database Access Layer
// All classes in DAL should be only access to DB
// All classes in DAL should only be accessibele from BLL

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;
using BlissBase.Model;

namespace BlissBase.DAL
{
    /// <summary>
    /// Class of CompositeSymbols table
    /// Primary Key CompID
    /// </summary>  
    public class CompositeSymbols
    {
        [Key]
        public int CompID { get; set; }
        [Required]
        public string CompName { get; set; }
        [Required]
        public byte[] CompJPEG { get; set; }
    }

    /// <summary>
    /// Class of Languages table
    /// Primary Key LangID
    /// Foreign Key CompID references CompositeSymbols
    /// Uses EnumDataType to store BlissBase.Model.LanguageCodes
    /// enum as int in LangCodeInt field
    /// </summary>
    public class Languages
    {
        // Reference used: http://stackoverflow.com/a/10804997
        [Key, ForeignKey("CompositeSymbols")]
        public int CompID { get; set; }
        public CompositeSymbols CompositeSymbols { get; set; }
        [Required]
        public virtual int LangCodeInt { 
            get
            {
                return (int)this.LangCode;
            }
            set
            {
                LangCode = (LanguageCodes)value;
            }
        }
        [EnumDataType(typeof(LanguageCodes))]
        public LanguageCodes LangCode { get; set; }
    }

    /// <summary>
    /// Class of CompositeOf table
    /// Primary Key CompOfID
    /// Foreign Key CompId references CompositeSymbols.CompID
    /// Foreign Key SymID references Symbols.SymID
    /// </summary>  
    public class CompositesOf
    {
        [Key]
        public int CompOfID { get; set; }
        public int CompID { get; set; }
        public CompositeSymbols CompositeSymbols { get; set; }
        public int SymID { get; set; }
        public Symbols Symbols { get; set; }
    }

    /// <summary>
    /// Class of Symbols table
    /// Primary Key SymID
    /// </summary>  
    public class Symbols
    {
        [Key]
        public int SymID { get; set; }
        [Required]
        public String SymName { get; set; }
        [Required]
        public byte[] SymJPEG { get; set; }
    }

    /// <summary>
    /// Class of Types table
    /// Primary Key TypeID
    /// Foreign Key SymID references Symbols
    /// Uses EnumDataType to store BlissBase.Model.TypeCodes
    /// enum as int in TypeIndicatorInt field
    /// </summary>
    public class SymbolTypes
    {
        // Reference used: http://stackoverflow.com/a/10804997
        [Key, ForeignKey("Symbols")]
        public int SymID { get; set; }
        public Symbols Symbols { get; set; }
        [Required]
        public virtual int TypeIndicatorInt {
            get
            {
                return (int)this.TypeIndicator;
            }
            set
            {
                TypeIndicator = (TypeCodes)value;
            }
        }
        [EnumDataType(typeof(TypeCodes))]
        public TypeCodes TypeIndicator { get; set; }
    }

    /// <summary>
    /// Class of Users table
    /// Primary key UserID
    /// </summary>
    public class Users
    {
        [Key]
        public int UserID { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public string UserFirstName { get; set; }
        [Required]
        public string UserLastName { get; set; }
        [Required]
        public string UserPasswd { get; set; }
        [Required]
        public string UserSalt { get; set; }
        [Required]
        public bool UserApproved { get; set; }

    }

    /// <summary>
    /// Class of Admins table
    /// Primary Key AdminID
    /// Foreign Key UserID references Users
    /// </summary>
    public class Admins
    {
        [Key, ForeignKey("Users")]
        public int UserID { get; set; }
        public Users Users { get; set; }
    } 

    /// <summary>
    /// Class of UsersSynonymsList table
    /// Primary Key UListID
    /// Foreign Key UserID references Users.UserID
    /// </summary>  
    public class UsersSynonymsList
    {
        [Key, ForeignKey("Users")]
        public int UserID { get; set; }
        public Users Users { get; set; }
    }

    /// <summary>
    /// Class of UsersSynonyms table
    /// Primary Key USynID
    /// Foreign Key USynWord references CompositeSymbols.CompID
    /// Foreign Key USynSynonym references CompositeSymbols.CompID
    /// </summary>
    public class UserSynonyms
    {
        [Key]
        public int USynID { get; set; }
        public int USynWord { get; set; }
        [ForeignKey("USynWord")]
        public CompositeSymbols CompositeSymbols { get; set; }
        public int USynSynonym { get; set; }
        [ForeignKey("USynSynonym")]
        public CompositeSymbols CompositeSynonyms { get; set; }
        public int UserID { get; set; }
        public UsersSynonymsList UsersSynonymsList { get; set; }
        [Required]
        public bool USynApproved { get; set; }
    }

    /// <summary>
    /// Class of GlobalSynonyms table
    /// Primary Key GSynID
    /// Foreign Key GSynWord references CompositeSymbols.CompID
    /// Foreign Key GSynSynonym references CompositeSymbols.CompID
    /// </summary>
    public class GlobalSynonyms
    {
        [Key]
        public int GSynID { get; set; }
        public int GSynWord { get; set; }
        [ForeignKey("GSynWord")]
        public CompositeSymbols CompositeSymbols { get; set; }
        public int GSynSynonym { get; set; }
        [ForeignKey("GSynSynonym")]
        public CompositeSymbols CompositeSynonyms { get; set; }
    }

    /// <summary>
    /// Class of CompositeScores table
    /// Primary Key CompID
    /// Foreign Key CompID references CompositeSymbols.CompID
    /// </summary>
    public class CompositeScores
    {
        [Key, ForeignKey("CompositeSymbols")]
        public int CompID { get; set; }
        public CompositeSymbols CompositeSymbols { get; set; }
        [Required]
        public int CompScore { get; set; }
    }

    /// <summary>
    /// Class of RawSymbolsImport table
    /// Primary Key RawID
    /// </summary>
    public class RawSymbolsImport
    {
        [Key]
        public int RawID { get; set; }
        [Required]
        public string RawName { get; set; }
        [Required]
        public byte[] RawJPEG { get; set; }
    }


    // BlissBaseContext : DBContext
    // Defines all fields in all tables in database BlissBase
    // Creates database if not exists
    // Info about the tables can be found in there corresponding
    // [table]DAL.cs file
    public class BlissBaseContext : DbContext
    {
        /// <summary>
        /// Need default constructor for "entety framework power tools beta4" plugin
        /// </summary>
        public BlissBaseContext()
        {
            try
            {
                this.Database.CreateIfNotExists();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Constructor that takes a bool testing parameter.
        /// BLL should normally use false, only  when executing code from
        /// BlissBase.UT should the bool be true.
        /// </summary>
        /// <param name="testing"></param>
        public BlissBaseContext(bool testing)
            : base("name=BlissBase" + (testing ? "Testing" : "Context"))
        {
            try
            {
                this.Database.CreateIfNotExists();
            }
            catch (Exception e)
            {
                Debug.WriteLine("Create If Not Exists Failed with Exception");
                Debug.WriteLine(e.StackTrace);
            }
        }

        public DbSet<CompositeSymbols> CompositeSymbols { get; set; }
        public DbSet<Languages> Languages { get; set; }
        public DbSet<CompositesOf> CompositesOf { get; set; }
        public DbSet<Symbols> Symbols { get; set; }
        public DbSet<SymbolTypes> SymbolTypes { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UsersSynonymsList> UsersSynonymsList { get; set; }
        public DbSet<Admins> Admins { get; set; }
        public DbSet<UserSynonyms> UserSynonyms { get; set; }
        public DbSet<GlobalSynonyms> GlobalSynonyms { get; set; }
        public DbSet<CompositeScores> CompositeScores { get; set; }
        public DbSet<RawSymbolsImport> RawSymbolsImport { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();

            // If a row in [User/Global]Synonyms is deleted, there is not necessarily
            // any reason to delete the composite symbol that is the "synonym" or "word"
            // Therefore "OneToManyCascadeDeleteConvention" is disabeled
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
        }

    }
}

/* REFERENCES USED IN THIS FILE
 * http://www.entityframeworktutorial.net/code-first/foreignkey-dataannotations-attribute-in-code-first.aspx
 * http://forums.asp.net/t/1943430.aspx?Entity+frame+work+code+first+saving+image+in+database
 * https://msdn.microsoft.com/en-us/data/jj591620.aspx
 * https://msdn.microsoft.com/en-us/library/jj653752%28v=vs.110%29.aspx
 * Used for all of DAL
 * Capitalization Styles
 * https://msdn.microsoft.com/en-us/library/x2dbyw72%28v=vs.71%29.aspx
*/