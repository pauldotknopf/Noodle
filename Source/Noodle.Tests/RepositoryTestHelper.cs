using System;
using System.Collections.Generic;
using Ninject;
using Noodle.Collections;
using Noodle.Data;

namespace Noodle.Tests
{
    public class RepositoryTestHelper<T> where T:BaseEntity, new()
    {
        private readonly IKernel _kernel;
        private readonly Func<T, int> _hashCode;
        private readonly Func<int, T> _create;
        private T _instance;
        private Func<T, bool> _isDeleted;

        public RepositoryTestHelper(IKernel kernel, Func<T, int> hashCode, Func<int, T> create, Func<T, bool> isDeleted = null)
        {
            _isDeleted = isDeleted ?? ((item) => item == null);
            _kernel = kernel;
            _hashCode = hashCode;
            _create = create;
        }

        private void CanInsert()
        {
            _instance = _create(1);
            _kernel.Get<IRepository<T>>().Insert(_instance);
            (_instance.Id > 0).ShouldBeTrue();
        }

        private void CanUpdate()
        {
            if(_instance == null)
                CanInsert();

            var id = _instance.Id;

            _instance = _create(2);
            _instance.Id = id;

            _kernel.Get<IRepository<T>>().Update(_instance);

            var dbInstance = _kernel.Get<IRepository<T>>().GetById(_instance.Id);

            Comparer().Equals(dbInstance, _instance).ShouldBeTrue();
        }
        
        private void CanDelete()
        {
            if (_instance == null)
                CanInsert();

            _kernel.Get<IRepository<T>>().Delete(_instance);

            var dbInstance = _kernel.Get<IRepository<T>>().GetById(_instance.Id);

            _isDeleted(dbInstance).ShouldBeTrue();
        }

        public void CanInsertUpdateDelete()
        {
            var existingTime = CommonHelper.CurrentTime;
            var time = DateTime.UtcNow;
            CommonHelper.CurrentTime = () => time;

            CanInsert();
            CanUpdate();
            CanDelete();

            CommonHelper.CurrentTime = existingTime;
        }

        public IEqualityComparer<T> Comparer()
        {
            return new DelegateEqualityComparer<T>((t1, t2) => _hashCode(t1).Equals(_hashCode(t2)), t => _hashCode(t));
        } 
    }
}
