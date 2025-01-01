using System;
using System.Collections.Generic;
using SyntacticSugar;

public interface IPyObject
{
    // 表示
    string __repr__();
    string __str__();

    // 哈希值
    long __hash__();

    string __format__(string format);

    // 容器协议
    long __len__();
    
    
    IPyObject __getattribute__(string name);

    
}