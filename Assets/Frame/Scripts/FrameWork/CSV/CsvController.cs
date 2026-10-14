using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class CsvController  {

	static CsvController csv;
	public List<string[]> arrayData;  

	private CsvController()   //单例，构造方法为私有
	{
		arrayData = new List<string[]>();
	}

	public static CsvController GetInstance()   //单例方法获取对象
	{
		if(csv == null)
		{
			csv = new CsvController();
		}
		return csv;
	}

	public void loadFile(string path,string fileName)
	{
		arrayData.Clear();
		StreamReader sr = null;
		try
		{
			string file_url = path + "//" + fileName;    
			sr = File.OpenText(file_url);
		}
		catch
		{
			return; 
		}

		string line;
		while((line = sr.ReadLine()) != null)   
		{
			arrayData.Add(line.Split(',')); 
		}
		sr.Close();
		sr.Dispose();
	}

	public string getString(int row,int col)
	{
		return arrayData[row][col];
	}
	public int getInt(int row,int col)
	{
		return int.Parse(arrayData[row][col]);
	}
}
