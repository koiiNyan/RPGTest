using Newtonsoft.Json.Linq;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using UnityEngine;

namespace RPG.Units
{
    public abstract class UnitInputComponent : UnitComponent
    {
        private Dictionary<string, FieldInfo> _events = new Dictionary<string, FieldInfo>();
        private Func<string, Delegate[]> _expression;


        protected Vector3 _movement;

        public ref Vector3 MoveDirection => ref _movement;

        public SimpleHandle MainEventHandler;
        public SimpleHandle AdditionalEventHandler;
        public SimpleHandle TargetEventHandler;

        protected override void Awake()
        {
            base.Awake();
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(t => t.FieldType == typeof(SimpleHandle));
            foreach (var field in fields)
                _events.Add(field.Name, field);

            var strExpr = Expression.Parameter(typeof(string));
            var dicConstExpr = Expression.Constant(_events, typeof(Dictionary<string, FieldInfo>));

            var valueExpr = Expression.Property(dicConstExpr, typeof(Dictionary<string, FieldInfo>).GetProperty("Item"), strExpr);
            var fieldExpr = Expression.Call(valueExpr, typeof(FieldInfo).GetMethod(nameof(FieldInfo.GetValue)), Expression.Constant(this));
            var delegates = Expression.Convert(fieldExpr, typeof(MulticastDelegate));

            var methodInfo = typeof(MulticastDelegate).GetMethod(nameof(MulticastDelegate.GetInvocationList));
            var GetInvocationListExpr = Expression.Call(delegates, methodInfo);

            var arrayExpr = Expression.Convert(GetInvocationListExpr, typeof(Delegate[]));
            _expression = Expression.Lambda<Func<string, Delegate[]>>(arrayExpr, strExpr).Compile();
        }

        protected void CallSimpleHandle(string name)
        {
            foreach (var @event in _expression.Invoke(name))
            {
                @event.Method.Invoke(@event.Target, null);
            }
        }
	}
}
