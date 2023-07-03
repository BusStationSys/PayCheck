namespace PayCheck.Business
{
    using System;
    using PayCheck.Infrastructure.UnitOfWork.Interfaces;
    using AutoMapper;

    public abstract class BaseBusiness : IDisposable
    {
        private bool _disposedValue = false;

        protected IUnitOfWork _unitOfWork;

        protected Mapper _mapper;

        protected IUnitOfWork UnitOfWork
        {
            get
            {
                return this._unitOfWork;
            }
        }

        protected BaseBusiness(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _disposedValue = true;
            }
        }
    }
}