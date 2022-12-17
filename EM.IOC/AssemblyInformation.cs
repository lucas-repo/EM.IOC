using EM.Bases;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace EM.IOC
{
    /// <summary>
    /// 程序集信息
    /// </summary>
    public abstract class AssemblyInformation: BaseCopy, IAssemblyInformation
    {
        #region Fields

        private Type _classType;

        private FileVersionInfo _file;

        #endregion

        #region Properties

        public virtual string AssemblyQualifiedName
        {
            get
            {
                string fullName = string.Empty;
                try
                {
                    fullName = ReferenceType.AssemblyQualifiedName;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }

                return fullName;
            }
        }

        public virtual string Author
        {
            get
            {
                string author = string.Empty;
                try
                {
                    author = ReferenceFile.CompanyName;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }

                return author;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string BuildDate
        {
            get
            {
                string buildDate = string.Empty;
                try
                {
                    buildDate = File.GetLastWriteTime(ReferenceAssembly.Location).ToLongDateString();
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }

                return buildDate;
            }
        }

        public virtual string Description
        {
            get
            {
                string desc = string.Empty;
                try
                {
                    desc = ReferenceFile.Comments;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }

                return desc;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual string Name
        {
            get
            {
                string name = string.Empty;
                try
                {
                    name = ReferenceFile.ProductName;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }

                return name;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public virtual string Version
        {
            get
            {
                string version = string.Empty;
                try
                {
                    version = ReferenceFile.FileVersion;
                }
                catch (Exception e)
                {
                    Trace.WriteLine(e);
                }

                return version;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        protected Assembly ReferenceAssembly
        {
            get
            {
                if (_classType == null) _classType = GetType();

                return _classType.Assembly;
            }
        }

        protected FileVersionInfo ReferenceFile
        {
            get
            {
                return _file ?? (_file = FileVersionInfo.GetVersionInfo(ReferenceAssembly.Location));
            }
        }

        protected Type ReferenceType => _classType ?? (_classType = GetType());

        #endregion
    }
}