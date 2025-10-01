//using System;
//using System.Collections.Generic;
//namespace Johnny.SimDungeon
//{
//    public class ObjectPool<T> where T : class, new()
//    {
//        private readonly Stack<T> pool;          // 存放对象的栈
//        private readonly Func<T> factory;        // 工厂方法，创建新对象
//        private readonly Action<T> onGet;        // 对象取出时的回调
//        private readonly Action<T> onRelease;    // 对象回收时的回调
//        private readonly int maxSize;            // 最大容量（0 表示无限）

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
//        /// 从池里取对象
//        /// </summary>
//        public T Get()
//        {
//            var item = pool.Count > 0 ? pool.Pop() : factory();
//            onGet?.Invoke(item);
//            return item;
//        }

//        /// <summary>
//        /// 回收对象
//        /// </summary>
//        public void Release(T item)
//        {
//            if (maxSize > 0 && pool.Count >= maxSize)
//            {
//                // 超出最大容量就直接丢弃
//                return;
//            }

//            onRelease?.Invoke(item);
//            pool.Push(item);
//        }

//        /// <summary>
//        /// 预加载指定数量的对象
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