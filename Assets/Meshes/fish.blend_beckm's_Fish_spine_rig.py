#----------------------------------------------------------
#written by Alec Farai :)
# Fish spine rig .py 
# got to https://www.youtube.com/watch?v=MrN4U3PlQbE for tutorial
#
#
#     ⇓⇓⇓⇓⇓  Animators codding area is at the bottom  ⇓⇓⇓⇓⇓⇓ ↵
#
# 
#----------------------------------------------------------
import bpy 
from bpy.props import *
import math 
import time as t
from os import path
import os
filepath = os.path.dirname(os.path.realpath(__file__)) 
username = os.getenv('USERNAME')
script_path = os.path.realpath(__file__)
pathsize = list(filepath)
proccesor = os.getenv('PROCESSOR_ARCHITECTURE')
bpy.context.space_data.show_word_wrap = True
#
# saves up back up script at your current folder location if you dont remember were you put it
#
def createFile(dest):
    if(len(pathsize) >3):
        date = t.localtime(t.time())
        name = 'READ ME %d_%d_%d.txt'%(date[2],date[1],date[0])
        f = open(dest+name,"w")
        f.write( "/////// some Data just for fun /////// ")
        f.write( "/////// temporary fish spine tool script saved /////// ")
        f.write( "\n/////// file saved at  "+filepath)
        f.write ("\n//User name is "+username)
        f.write( "\n//last edited script on %d_%d_%d"%(date[2],date[1],date[0]))
        f.write( "\n//your text script is at this location >> "+script_path)
        f.write ("\n//proccessor is of type "+ proccesor)
        f.write ("\n//proccessor is a "+(os.getenv('PROCESSOR_IDENTIFIER')))
        if "AMD" in proccesor:
            f.write ("\n//AMD is ok but Currently NVidia with CUDA renders faster.")
        if  "IA64" in proccesor:
            f.write ("\n// This architecture is very powerful, designed for very high end servers.")         
        f.close()    
if __name__ == '__main__':
    destination = filepath
    createFile(destination)
    
def save_backup_file():
     if(len(pathsize) > 3):  
        bpy.ops.text.save_as(filepath=(filepath+"_"+username+"'s_Fish_spine_rig.py"), check_existing=True, 
        filter_blender=False, filter_backup=False, filter_image=False, filter_movie=False, 
        filter_python=True, filter_font=False, filter_sound=False, filter_text=True, 
        filter_btx=False, filter_collada=False, filter_folder=True, filemode=9)#saves back up script  

save_backup_file()#remove this line if you dont want
                  # the script to save a backup file in your folder

#
#  Store properties in the active scene
#
def initSceneProperties(scn):
 
    bpy.types.Scene.MyAmpFloat = FloatProperty(
        name = "Amplitude", 
        description = "Enter a float",
        default = 0.05,
        min = -100,
        max = 100)
        
    bpy.types.Scene.MySpeedFloat = FloatProperty(
        name = "Frequency", 
        description = "Enter a float",
        default = 0.85,
        min = -100,
        max = 100)
        
    bpy.types.Scene.MyEnum = EnumProperty(
        items = [('Xaxis', 'X_axis', 'Xaxis'), #you can name these strings whatever you prefer
                 ('Yaxis', 'Y_axis', 'Yaxis'),
                 ('Zaxis', 'Z_axis', 'Zaxis')],
        name = "rotation_euler axis")
    scn['MyEnum'] = 2 
    return
 
initSceneProperties(bpy.context.scene)

#
#    Menu in UI region
#
class UIPanel(bpy.types.Panel):
    bl_label = " Fish Spine rig panel"
    bl_space_type = "VIEW_3D"
    bl_region_type = "TOOLS"
 
    def draw(self, context):
        layout = self.layout
        scn = context.scene
        
        layout = layout.column(align=True)
        layout.prop(scn, 'MyAmpFloat')
        layout.prop(scn, 'MySpeedFloat')
        layout.prop(scn, 'MyEnum')

        self.layout.operator("reset.reset", text='Reset ',icon="RECOVER_AUTO").reset = "reset"
        self.layout.operator("reset.reset", text='run',icon='LOGIC').reset = "run"
            
class OBJECT_OT_ResetButton(bpy.types.Operator):
    bl_idname = "reset.reset"
    bl_label = "Say Hello"
    reset = bpy.props.StringProperty()
 
    def execute(self, context):
        if self.reset == 'reset':
            for i in range (-1,len(objlist)):
                bpy.ops.object.select_pattern(pattern=(objlist[i]))
            bpy.app.handlers.frame_change_pre.clear()
            bpy.ops.object.rotation_clear()
            bpy.ops.screen.animation_cancel()
            bpy.context.scene.MyAmpFloat = bpy.context.scene.MyAmpFloat
            bpy.context.scene.MySpeedFloat = bpy.context.scene.MySpeedFloat

        elif self.reset == 'run':
            bpy.app.handlers.frame_change_pre.append(swim_action)
            bpy.ops.screen.animation_play()
    
        return{'FINISHED'} 
#
#    The button prints the values of the properites in the console.
#       ⇓⇓⇓⇓⇓
 
class OBJECT_OT_PrintPropsButton(bpy.types.Operator):
    bl_idname = "idname_must.be_all_lowercase_and_contain_one_dot"
    bl_label = "Print props"
 
    def execute(self, context):
        scn = context.scene
        printProp("Float:  ", 'MySpeedFloat', scn)
        printProp("Float:  ", 'MyAmpFloat', scn) 
        printProp("Enum:   ", 'MyEnum', scn)
        return{'FINISHED'}    
 
def printProp(label, key, scn):
    try:
        val = scn[key]
    except:
        val = 'Undefined'
    print("%s %s" % (key, val))

def swim_action(scene):
    frame = scene.frame_current
    speed = bpy.context.scene['MySpeedFloat']
    amplitude =  bpy.context.scene['MyAmpFloat']
    axis = bpy.context.scene['MyEnum']                         
    ofset = range(len(objlist)+1)     
    for i in range (-1,len(objlist)):#
        i+=1                        #
        cube = bpy.data.objects[objlist[i]]
        if frame > 0:
            cube.rotation_euler[axis] = math.sin((frame*speed)+ofset[-i])*amplitude       

bpy.app.handlers.frame_change_pre.append(swim_action)# for every frame change it will execute the swim_action def:
#    Registration
bpy.utils.register_module(__name__)
bpy.ops.text.move(type='FILE_BOTTOM')# Pushes your cursor to the bottom of the page every time you run it
'''
Animators codding area below

remember to save your .blend file
'''
#always end the list with a comma ,
proxies= 'Cube,Cube1,Cube2,Cube3,Cube4,Cube5'#list of objects to attach the expression to
objlist = proxies.split(",")#splits every word as an object wherever theres a comma ,
#FYI Amplitude is the how large the Wave is And Frequency is the speed of it
#for tutorial >> https://www.youtube.com/watch?v=MrN4U3PlQbE
