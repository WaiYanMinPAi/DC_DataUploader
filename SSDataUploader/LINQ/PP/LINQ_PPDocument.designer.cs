﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SSDataUploader.LINQ.PP
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[global::System.Data.Linq.Mapping.DatabaseAttribute(Name="PP_Document")]
	public partial class LINQ_PPDocumentDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertSysImage(SysImage instance);
    partial void UpdateSysImage(SysImage instance);
    partial void DeleteSysImage(SysImage instance);
    #endregion
		
		public LINQ_PPDocumentDataContext() : 
				base(global::SSDataUploader.Properties.Settings.Default.PP_DocumentConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public LINQ_PPDocumentDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LINQ_PPDocumentDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LINQ_PPDocumentDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public LINQ_PPDocumentDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<SysImage> SysImages
		{
			get
			{
				return this.GetTable<SysImage>();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.TableAttribute(Name="dbo.SysImage")]
	public partial class SysImage : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _ImageID;
		
		private System.Data.Linq.Binary _ImageData;
		
		private bool _Active;
		
		private string _CreatedBy;
		
		private System.DateTime _CreatedOn;
		
		private string _ModifiedBy;
		
		private System.DateTime _ModifiedOn;
		
		private string _LastAction;
		
		private string _ImageRecordType;
		
		private string _Imagestr;
		
		private string _PersonalNo;
		
		private System.Nullable<System.DateTime> _UploadedOn;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnImageIDChanging(string value);
    partial void OnImageIDChanged();
    partial void OnImageDataChanging(System.Data.Linq.Binary value);
    partial void OnImageDataChanged();
    partial void OnActiveChanging(bool value);
    partial void OnActiveChanged();
    partial void OnCreatedByChanging(string value);
    partial void OnCreatedByChanged();
    partial void OnCreatedOnChanging(System.DateTime value);
    partial void OnCreatedOnChanged();
    partial void OnModifiedByChanging(string value);
    partial void OnModifiedByChanged();
    partial void OnModifiedOnChanging(System.DateTime value);
    partial void OnModifiedOnChanged();
    partial void OnLastActionChanging(string value);
    partial void OnLastActionChanged();
    partial void OnImageRecordTypeChanging(string value);
    partial void OnImageRecordTypeChanged();
    partial void OnImagestrChanging(string value);
    partial void OnImagestrChanged();
    partial void OnPersonalNoChanging(string value);
    partial void OnPersonalNoChanged();
    partial void OnUploadedOnChanging(System.Nullable<System.DateTime> value);
    partial void OnUploadedOnChanged();
    #endregion
		
		public SysImage()
		{
			OnCreated();
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ImageID", DbType="NVarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string ImageID
		{
			get
			{
				return this._ImageID;
			}
			set
			{
				if ((this._ImageID != value))
				{
					this.OnImageIDChanging(value);
					this.SendPropertyChanging();
					this._ImageID = value;
					this.SendPropertyChanged("ImageID");
					this.OnImageIDChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ImageData", DbType="VarBinary(MAX)", UpdateCheck=UpdateCheck.Never)]
		public System.Data.Linq.Binary ImageData
		{
			get
			{
				return this._ImageData;
			}
			set
			{
				if ((this._ImageData != value))
				{
					this.OnImageDataChanging(value);
					this.SendPropertyChanging();
					this._ImageData = value;
					this.SendPropertyChanged("ImageData");
					this.OnImageDataChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Active", DbType="Bit NOT NULL")]
		public bool Active
		{
			get
			{
				return this._Active;
			}
			set
			{
				if ((this._Active != value))
				{
					this.OnActiveChanging(value);
					this.SendPropertyChanging();
					this._Active = value;
					this.SendPropertyChanged("Active");
					this.OnActiveChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreatedBy", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string CreatedBy
		{
			get
			{
				return this._CreatedBy;
			}
			set
			{
				if ((this._CreatedBy != value))
				{
					this.OnCreatedByChanging(value);
					this.SendPropertyChanging();
					this._CreatedBy = value;
					this.SendPropertyChanged("CreatedBy");
					this.OnCreatedByChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_CreatedOn", DbType="DateTime NOT NULL")]
		public System.DateTime CreatedOn
		{
			get
			{
				return this._CreatedOn;
			}
			set
			{
				if ((this._CreatedOn != value))
				{
					this.OnCreatedOnChanging(value);
					this.SendPropertyChanging();
					this._CreatedOn = value;
					this.SendPropertyChanged("CreatedOn");
					this.OnCreatedOnChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ModifiedBy", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string ModifiedBy
		{
			get
			{
				return this._ModifiedBy;
			}
			set
			{
				if ((this._ModifiedBy != value))
				{
					this.OnModifiedByChanging(value);
					this.SendPropertyChanging();
					this._ModifiedBy = value;
					this.SendPropertyChanged("ModifiedBy");
					this.OnModifiedByChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ModifiedOn", DbType="DateTime NOT NULL")]
		public System.DateTime ModifiedOn
		{
			get
			{
				return this._ModifiedOn;
			}
			set
			{
				if ((this._ModifiedOn != value))
				{
					this.OnModifiedOnChanging(value);
					this.SendPropertyChanging();
					this._ModifiedOn = value;
					this.SendPropertyChanged("ModifiedOn");
					this.OnModifiedOnChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_LastAction", DbType="NVarChar(50) NOT NULL", CanBeNull=false)]
		public string LastAction
		{
			get
			{
				return this._LastAction;
			}
			set
			{
				if ((this._LastAction != value))
				{
					this.OnLastActionChanging(value);
					this.SendPropertyChanging();
					this._LastAction = value;
					this.SendPropertyChanged("LastAction");
					this.OnLastActionChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_ImageRecordType", DbType="NVarChar(50)")]
		public string ImageRecordType
		{
			get
			{
				return this._ImageRecordType;
			}
			set
			{
				if ((this._ImageRecordType != value))
				{
					this.OnImageRecordTypeChanging(value);
					this.SendPropertyChanging();
					this._ImageRecordType = value;
					this.SendPropertyChanged("ImageRecordType");
					this.OnImageRecordTypeChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Imagestr", DbType="NVarChar(MAX)")]
		public string Imagestr
		{
			get
			{
				return this._Imagestr;
			}
			set
			{
				if ((this._Imagestr != value))
				{
					this.OnImagestrChanging(value);
					this.SendPropertyChanging();
					this._Imagestr = value;
					this.SendPropertyChanged("Imagestr");
					this.OnImagestrChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_PersonalNo", DbType="NVarChar(50)")]
		public string PersonalNo
		{
			get
			{
				return this._PersonalNo;
			}
			set
			{
				if ((this._PersonalNo != value))
				{
					this.OnPersonalNoChanging(value);
					this.SendPropertyChanging();
					this._PersonalNo = value;
					this.SendPropertyChanged("PersonalNo");
					this.OnPersonalNoChanged();
				}
			}
		}
		
		[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_UploadedOn", DbType="DateTime")]
		public System.Nullable<System.DateTime> UploadedOn
		{
			get
			{
				return this._UploadedOn;
			}
			set
			{
				if ((this._UploadedOn != value))
				{
					this.OnUploadedOnChanging(value);
					this.SendPropertyChanging();
					this._UploadedOn = value;
					this.SendPropertyChanged("UploadedOn");
					this.OnUploadedOnChanged();
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
}
#pragma warning restore 1591
