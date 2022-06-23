with Ada.Text_IO;

procedure Main is

   can_stop : boolean := false;
   pragma Atomic(can_stop);

   task type break_thread;
   task type main_thread(id: Integer, step: Integer);

   task body break_thread is
   begin
      delay 15.0;
      can_stop := true;
   end break_thread;

   task body main_thread is
      sum : Long_Long_Integer := 0;
      amount : Long_Long_Integer := 0;

   begin
      loop
         sum := sum + step;
         amount := amount + 1;
         exit when can_stop;
      end loop;
      delay 1.0;

      Ada.Text_IO.Put_Line(id'Img & " sum: " & sum'Img & " step: " & step'Img & " amount of steps: " & amount'Img);
   end main_thread;

   b1 : break_thread;
   t1 : main_thread(1, 1);
   t2 : main_thread(2, 2);
   t3 : main_thread(3, 3);
   t4 : main_thread(4, 4);
begin
   null;
end Main;