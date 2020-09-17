using System;
using System.Collections.Generic;

namespace GitGUI.Logic
{
    class A
    {
        public B B { get; } = new B();

        public A()
        {
            B.Clicked += OnClicked;
        }

        void OnClicked(C sender, EventArgs e)
        {

        }
    }

    class B
    {
        public List<C> Cs = new List<C>();

        public delegate void ClickedEventHandler(C c, EventArgs e);
        public event ClickedEventHandler Clicked;

        public void NewC()
        {
            C c = new C();
            c.Clicked += OnClicked;
            Cs.Add(c);
        }

        void OnClicked(C sender, EventArgs e)
        {
            Clicked?.Invoke(sender, e);
        }
    }

    class C
    {
        public delegate void ClickedEventHandler(C c, EventArgs e);
        public event ClickedEventHandler Clicked;

        public void OnClicked()
        {
            Clicked?.Invoke(this, new EventArgs());
        }
    }
}

namespace pokus
{
    class A
    {
        public B B { get; } = new B();

        public A()
        {
            B.SetCsHandler(OnClicked);
        }
        void OnClicked(C sender, EventArgs e)
        {

        }
    }

    class B
    {
        public List<C> Cs = new List<C>();
        C.ClickedEventHandler Handler { get; set; }

        public void SetCsHandler(C.ClickedEventHandler handler)
        {
            Handler = handler;
        }

        public void NewC()
        {
            C c = new C();
            c.Clicked += Handler;
            Cs.Add(c);
        }
    }

    class C
    {
        public delegate void ClickedEventHandler(C c, EventArgs e);
        public event ClickedEventHandler Clicked;

        public void OnClicked()
        {
            Clicked?.Invoke(this, new EventArgs());
        }
    }
}
