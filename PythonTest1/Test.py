import clr
clr.AddReference("System")
clr.AddReferenceToFile("IronPython.Modules.dll")

login_token = ''

def set_login_token(token):
    global login_token
    login_token = token

def make_login_id():
    login_id = 'test'
    login_id += str(proxy.Random(1, 100))
    return login_id

def HelloWorld():
    data = 'Hello World C# sdfadsfsdf'
    return data

def HelloWorld2(data):
    return data

def HelloWorld3():
    proxy.ShowMessage('called sdfsdlfksdklflksdklflk proxy.ShowMessage')

def ListTest():
    data = []
    data.append('Hello')
    data.append('World')
    data.append('black falcon')
    return data

class MyClass(object):
    def __init__(self, value):
        self.value = value

class Calculator:
   def add(self, argA, argB):
      return argA+argB
   def sub(self, argA, argB):
      return argA-argB