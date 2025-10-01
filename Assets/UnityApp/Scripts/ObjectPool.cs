//using System;
//using System.Collections.Generic;
//namespace Johnny.SimDungeon
//{
//    public class ObjectPool<T> where T : class, new()
//    {
//        private readonly Stack<T> pool;          // ��Ŷ����ջ
//        private readonly Func<T> factory;        // ���������������¶���
//        private readonly Action<T> onGet;        // ����ȡ��ʱ�Ļص�
//        private readonly Action<T> onRelease;    // �������ʱ�Ļص�
//        private readonly int maxSize;            // ���������0 ��ʾ���ޣ�

//        public int CountInactive => pool.Count;

//        public ObjectPool(Func<T> factory = null, Action<T> onGet = null, Action<T> onRelease = null, int maxSize = 0)
//        {
//            pool = new Stack<T>();
//            this.factory = factory ?? (() => new T());
//            this.onGet = onGet;
//            this.onRelease = onRelease;
//            this.maxSize = maxSize;
//        }

//        /// <summary>
//        /// �ӳ���ȡ����
//        /// </summary>
//        public T Get()
//        {
//            var item = pool.Count > 0 ? pool.Pop() : factory();
//            onGet?.Invoke(item);
//            return item;
//        }

//        /// <summary>
//        /// ���ն���
//        /// </summary>
//        public void Release(T item)
//        {
//            if (maxSize > 0 && pool.Count >= maxSize)
//            {
//                // �������������ֱ�Ӷ���
//                return;
//            }

//            onRelease?.Invoke(item);
//            pool.Push(item);
//        }

//        /// <summary>
//        /// Ԥ����ָ�������Ķ���
//        /// </summary>
//        public void Preload(int count)
//        {
//            for (var i = 0; i < count; i++)
//            {
//                var item = factory();
//                Release(item);
//            }
//        }
//    }
//}